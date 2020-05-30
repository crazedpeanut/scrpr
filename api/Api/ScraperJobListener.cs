using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Api.Models;
using Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Api
{
    public class ScraperJobListener : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ConnectionFactory connectionFactory;
        private readonly ILogger<ScraperJobProcessor> logger;
        private readonly ScrprConfiguration configuration;
        public ScraperJobListener(
            IServiceProvider serviceProvider,
            ConnectionFactory connectionFactory,
            ILogger<ScraperJobProcessor> logger,
            ScrprConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionFactory = connectionFactory;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: configuration.Queue.QueueNames.ScraperStart,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = JsonConvert.DeserializeObject<ScraperJob>(Encoding.UTF8.GetString(body.ToArray()));
                using var scope = serviceProvider.CreateScope();

                var jobProcessor = scope.ServiceProvider.GetRequiredService<ScraperJobProcessor>();

                try
                {
                    jobProcessor.Process(message, stoppingToken).Wait();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Unable to process message");
                }
            };
            channel.BasicConsume(
                queue: configuration.Queue.QueueNames.ScraperStart,
                autoAck: true,
                consumer: consumer);

            while(!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
    }
}