using Microsoft.Extensions.DependencyInjection;
using Scraper.Configuration;

namespace Scraper.Mongo
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, DatabaseConfiguration configuration) =>
            services
                .AddSingleton(Factories.MongoDatabaseFactory(configuration));

        public static IServiceCollection AddMongoCollection<T>(this IServiceCollection services, string name) =>
            services
                .AddSingleton(Factories.MongoCollectionFactory<T>(name));

        public static IServiceCollection AddRepository<T>(this IServiceCollection services, string name) where T : MongoEntity =>
            services
                .AddMongoCollection<T>(name)
                .AddSingleton<IRepository<T>, SimpleRepository<T>>();
    }
}