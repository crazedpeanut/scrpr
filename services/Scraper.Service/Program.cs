using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
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
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.${Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .AddEnvironmentVariables()
                .Build()
                .Bind(configuration);

            return Host
               .CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((env, builder) =>
               {
                   builder
                       .AddJsonFile("appsettings.json", true)
                       .AddJsonFile($"appsettings.${env.HostingEnvironment.EnvironmentName}.json", true)
                       .AddEnvironmentVariables();
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
                                   https.ServerCertificate =  new X509Certificate2(certData);
                               });
                           });
                       }));
        }

    }
}
