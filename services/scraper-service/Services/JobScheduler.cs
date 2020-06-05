using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using RabbitMQ.Client;
using Scraper;
using ScraperService.Models;
using shared;

namespace ScraperService.Services
{
    public class JobScheduler
    {
        private readonly ConnectionFactory connectionFactory;
        private readonly SharedConfiguration configuration;
        private readonly ScraperJobRepository jobRepository;
        public JobScheduler(
            ScraperJobRepository jobRepository,
            ConnectionFactory connectionFactory,
            SharedConfiguration configuration)
        {
            this.jobRepository = jobRepository;
            this.configuration = configuration;
            this.connectionFactory = connectionFactory;
        }

        public async Task<string> Begin(ScraperJob job)
        {
            await jobRepository.Create(job);

            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: configuration.Queue.QueueNames.ScraperStart,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var message = new ScraperJobQueueMessage
            {
                Id = job.Id,
                Url = job.Url.ToString()
            };

            var body = message.ToByteArray();

            channel.BasicPublish(
                exchange: "",
                routingKey: configuration.Queue.QueueNames.ScraperStart,
                basicProperties: null,
                body: body);

            return job.Id;
        }

        public Task<List<ScraperJob>> List()
        {
            return jobRepository.Get();
        }
    }
}