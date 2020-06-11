using System.Net.Mime;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Scraper.Configuration;
using Scraper.Mongo;
using Scraper.Service.Core.Models;

namespace Scraper.Service.Core.Services
{
    public class ScraperJobPublisher
    {
        private readonly ConnectionFactory connectionFactory;
        private readonly ScraperConfiguration configuration;
        private readonly IRepository<ScraperJob> jobRepository;

        public ScraperJobPublisher(
            IRepository<ScraperJob> jobRepository,
            ConnectionFactory connectionFactory,
            ScraperConfiguration configuration)
        {
            this.jobRepository = jobRepository;
            this.configuration = configuration;
            this.connectionFactory = connectionFactory;
        }

        public async Task<string> Publish(ScraperJob job)
        {
            await jobRepository.Create(job);

            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: configuration.Queue.ExchangeNames.ScraperStart,
                type: "topic",
                durable: true,
                autoDelete: false,
                arguments: null);

            var message = new ScraperStartMessage
            {
                JobId = job.Id,
                Collector = job.Collector
            };

            var body = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: configuration.Queue.ExchangeNames.ScraperStart,
                routingKey: $"{job.Collector.CollectorKind}.{job.Id}",
                basicProperties: null,
                body: body);

            return job.Id;
        }
    }
}