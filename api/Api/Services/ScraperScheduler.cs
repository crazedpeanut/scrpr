using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Api.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Api.Services
{
    public class ScraperScheduler
    {
        private readonly ScraperJobRepository jobRespository;
        private readonly ConnectionFactory connectionFactory;
        private readonly ScrprConfiguration configuration;
        public ScraperScheduler(ScraperJobRepository jobRespository, ConnectionFactory connectionFactory, ScrprConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionFactory = connectionFactory;
            this.jobRespository = jobRespository;
        }

        public async Task Begin(ScraperJob job, CancellationToken cancellationToken)
        {
            await jobRespository.Create(job, cancellationToken);

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

        public async Task<List<ScraperJob>> List(CancellationToken cancellationToken)
        {
            return await jobRespository.Get(cancellationToken);
        }
    }
}