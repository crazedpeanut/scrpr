using Microsoft.Extensions.DependencyInjection;
using Scraper.Configuration;
using Scraper.Mongo;
using Scraper.Service.Data.Models;

namespace Scraper.Service.Data
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, ScraperConfiguration configuration) =>
            services
                .AddMongoDb(configuration.Database)
                .AddRepository<ScraperSource>(configuration.Database.Collections.ScraperSources.Name);
    }
}