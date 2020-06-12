using System;
using System.Security.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scraper.Configuration;
using Scraper.Mongo;
using GraphQL.Server;
using Scraper.Api.Models.GraphQL;
using GraphQL.Types;
using GraphQL.Server.Ui.Altair;
using GraphQL.Server.Ui.GraphiQL;
using GraphQL.Server.Ui.Playground;
using GraphQL.Server.Ui.Voyager;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Grpc.Core;
using System.Threading.Tasks;

namespace Scraper.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            configuration.Bind(this.configuration);
            this.env = env;
        }

        public ScraperConfiguration configuration = new ScraperConfiguration();
        private readonly IHostEnvironment env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(
                options => options.AddDefaultPolicy(
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()));

            services.AddLogging();
            services.AddHttpClient();
            services.AddControllers();
            services.AddHttpContextAccessor();

            services
                .AddSingleton(configuration)
                .AddMongoDb(configuration.Database);

            services.AddSingleton<Services.ScraperService>(_ =>
            {
                var credentials = CallCredentials.FromInterceptor((context, metadata) =>
                {
                    metadata.Add("clientId", $"scraper-api");
                    return Task.CompletedTask;
                });

                var channelOptions = new GrpcChannelOptions
                {
                    Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
                };

                if (env.IsDevelopment())
                {
                    var httpClientHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };

                    channelOptions.HttpClient = new HttpClient(httpClientHandler);
                }

                var channel = GrpcChannel.ForAddress(configuration.Services.Scraper.BaseUrl, channelOptions);
                var client = new Scraper.ScraperService.ScraperServiceClient(channel);
                
                return new Services.ScraperService(client);
            });

            services
                .AddSingleton(SchemaBuilder.Create)
                .AddGraphQL((provider, options) =>
                {
                    options.EnableMetrics = true;
                    options.ExposeExceptions = env.IsDevelopment();
                    var logger = provider.GetRequiredService<ILogger<Startup>>();
                    options.UnhandledExceptionDelegate = (context) => logger.LogError(context.Exception, "GraphQL Error");
                })
                .AddGraphTypes()
                .AddSystemTextJson()
                .AddUserContextBuilder(httpContext => new GraphQLUserContext { User = httpContext.User });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseGraphQLAltair(new GraphQLAltairOptions());
                app.UseGraphiQLServer(new GraphiQLOptions());
                app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
                app.UseGraphQLVoyager(new GraphQLVoyagerOptions());
            }

            app.UseCors();

            app.UseRouting();

            app.UseAuthorization();

            app.UseGraphQL<ISchema>();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
