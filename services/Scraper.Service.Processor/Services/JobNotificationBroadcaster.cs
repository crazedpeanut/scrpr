using System.Threading.Tasks;
using RabbitMQ.Client;
using Scraper.Service.Data.Models;
using Shared;
using Google.Protobuf;

namespace Scraper.Service.Processor.Services
{
    public class JobNotificationBroadcaster
    {
        private readonly SharedConfiguration configuration;
        private readonly ConnectionFactory connectionFactory;
        public JobNotificationBroadcaster(SharedConfiguration configuration, ConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
            this.configuration = configuration;
        }

        public void Emit(ScraperJob job)
        {
            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: configuration.Queue.ExchangeNames.ScraperJobStatus,
                type: "topic",
                durable: true,
                autoDelete: false,
                arguments: null);

            var message = new ScraperJobChangeNotification
            {
                Id = job.Id,
                Url = job.Url.ToString(),
                Status = (ScraperJobStatus)job.Status
            };

            var body = message.ToByteArray();

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: configuration.Queue.ExchangeNames.ScraperJobStatus,
                routingKey: $"{job.Id}",
                basicProperties: null,
                body: body);
        }
    }
}