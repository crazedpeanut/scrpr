using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Scraper.Service.Data;
using Scraper.Service.Processor.Services;
using Shared;

namespace Scraper.Service.Processor
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new SharedConfiguration();
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .AddEnvironmentVariables()
                .Build()
                .Bind(config);

            return Host.CreateDefaultBuilder(args)
              .ConfigureServices((services) =>
              {
                  services
                  .AddHostedService<Worker>()
                  .AddSingleton(new ConnectionFactory() { HostName = config.Queue.Host })
                  .AddDataServices(config)
                  .AddSingleton<ScraperFactory>()
                  .AddSingleton<IEnumerable<IEntityExtractor>>(new List<IEntityExtractor>
                  {
                        new PhoneNumberEntityExtractor()
                  });
              });
        }
    }
}
