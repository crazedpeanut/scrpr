using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using ScraperService.Models;
using shared;

namespace scraper_service.Services
{
    public class JobQueue
    {
        private readonly ConnectionFactory connectionFactory;
        private readonly SharedConfiguration configuration;
        public JobQueue(ConnectionFactory connectionFactory, SharedConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionFactory = connectionFactory;
        }

        public void Queue(ScraperJob job)
        {
            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: configuration.Queue.QueueNames.ScraperStart,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            string message = JsonConvert.SerializeObject(job);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "",
                routingKey: configuration.Queue.QueueNames.ScraperStart,
                basicProperties: null,
                body: body);
        }
    }
}