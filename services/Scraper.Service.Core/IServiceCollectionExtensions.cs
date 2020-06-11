using Microsoft.Extensions.DependencyInjection;
using Scraper.Configuration;
using Scraper.Mongo;
using Scraper.Service.Core.Models;

namespace Scraper.Service.Core
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, ScraperConfiguration configuration) =>
            services
                .AddMongoDb(configuration.Database)
                .AddRepository<ScraperSource>(configuration.Database.Collections.ScraperSources.Name)
                .AddRepository<ScraperJob>(configuration.Database.Collections.ScraperJobs.Name);
    }
}