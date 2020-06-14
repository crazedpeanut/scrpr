using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Scraper.Configuration;
using Scraper.Service.Core;
using Scraper.Service.Core.Services;

namespace Scraper.Service.Scheduler
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ScraperConfiguration();
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .AddEnvironmentVariables()
                .Build()
                .Bind(config);

            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(config)
                        .AddHostedService<Worker>()
                        .AddSingleton(new ConnectionFactory() { HostName = config.Queue.Host })
                        .AddDataServices(config)
                        .AddSingleton<ScraperJobPublisher>()
                        .AddScoped<JobScheduler>();
                });
        }
    }
}
