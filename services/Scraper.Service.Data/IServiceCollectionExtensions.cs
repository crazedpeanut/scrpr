using BeetleX.Redis;
using Microsoft.Extensions.DependencyInjection;
using Scraper.Service.Data.Services;
using Shared;

namespace Scraper.Service.Data
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, SharedConfiguration configuration)
        {
            services
                .AddSingleton(configuration)
                .AddSingleton(new RedisDB(0, new JsonFormater()))
                .AddScoped<ScraperResultRepository>()
                .AddScoped<ScraperJobRepository>();

            return services;
        }
    }
}