using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Scraper.Configuration;

namespace Scraper.Service
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = new ScraperConfiguration();
            new ConfigurationBuilder()
                .AddScrpr()
                .Build()
                .Bind(configuration);

            return Host
               .CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((env, builder) =>
               {
                   builder
                    .AddScrpr();
               })
               .ConfigureWebHostDefaults(webBuilder =>
                   webBuilder
                       .UseStartup<Startup>()
                       .ConfigureKestrel(kestrel =>
                       {
                           kestrel.ListenAnyIP(5443, listen =>
                           {
                               listen.UseHttps(https =>
                               {
                                   var certData = Convert.FromBase64String(configuration.Services.Scraper.Certificate);
                                   https.ServerCertificate = new X509Certificate2(certData);
                               });
                           });
                       }));
        }
    }
}
