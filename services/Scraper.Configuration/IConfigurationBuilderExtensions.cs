using System;
using Microsoft.Extensions.Configuration;

namespace Scraper.Configuration
{
    public static class IConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddScrpr(this IConfigurationBuilder builder)
        {
            return builder
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.${Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .AddEnvironmentVariables("SCRPR_");
        }
    }
}
