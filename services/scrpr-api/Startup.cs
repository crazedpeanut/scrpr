using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using ScrprApi.Models;
using ScrprApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace ScrprApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            configuration.Bind(Configuration);
        }

        public ScrprConfiguration Configuration { get; } = new ScrprConfiguration();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(
                options => options.AddDefaultPolicy(
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()));

            services.AddHttpClient();
            services.AddControllers();

            services
                .AddSingleton(Configuration)
                .AddHostedService<ScraperJobListener>()
                .AddSingleton(new RabbitMQ.Client.ConnectionFactory() { HostName = Configuration.Queue.Host })
                .AddSingleton(MongoDatabaseFactory(Configuration.Database))
                .AddSingleton(MongoCollectionFactory<ScraperResult>(Configuration.Database.Collections.ScraperResult))
                .AddSingleton(MongoCollectionFactory<ScraperJob>(Configuration.Database.Collections.ScraperJob))
                .AddScoped<ScraperScheduler>()
                .AddScoped<ScraperJobRepository>()
                .AddScoped<ScraperJobProcessor>()
                .AddScoped<ScraperService>()
                .AddScoped<ScraperResultRespository>()
                .AddScoped<ScraperFactory>()
                .AddSingleton<IEnumerable<IEntityExtractor>>(new List<IEntityExtractor>(){
                    new PhoneNumberEntityExtractor()
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private static Func<IServiceProvider, IMongoCollection<T>> MongoCollectionFactory<T>(string name) => (IServiceProvider serviceProvider) => serviceProvider.GetRequiredService<IMongoDatabase>().GetCollection<T>(name);
        private static Func<IServiceProvider, IMongoDatabase> MongoDatabaseFactory(DatabaseConfiguration configuration) => (_) =>
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
