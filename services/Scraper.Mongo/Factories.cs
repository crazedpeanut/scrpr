using System;
using System.Security.Authentication;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Scraper.Configuration;

namespace Scraper.Mongo
{
    public static class Factories
    {
        public static Func<IServiceProvider, IMongoCollection<T>> MongoCollectionFactory<T>(string name) => (IServiceProvider serviceProvider) => serviceProvider.GetRequiredService<IMongoDatabase>().GetCollection<T>(name);
        public static Func<IServiceProvider, IMongoDatabase> MongoDatabaseFactory(DatabaseConfiguration configuration) => (_) =>
        {
            var settings = new MongoClientSettings();
            if (configuration.Host != null)
            {
                settings.Server = new MongoServerAddress(
                  configuration.Host,
                  configuration.Port
                );
            }

            if (configuration.Username != null)
            {
                settings.Credential = new MongoCredential(
                  "SCRAM-SHA-1",
                  new MongoInternalIdentity(
                    configuration.Name,
                    configuration.Username
                  ),
                  new PasswordEvidence(configuration.Password)
                );
            }

            if (configuration.UseTls)
            {
                settings.UseTls = true;
                settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };
            }

            return new MongoClient(settings).GetDatabase(configuration.Name);
        };
    }
}