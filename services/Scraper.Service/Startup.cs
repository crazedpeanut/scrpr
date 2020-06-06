using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Scraper.Service.Data;
using Scraper.Service.Services;
using Shared;

namespace Scraper.Service
{
    public class Startup
    {
        private readonly SharedConfiguration configuration = new SharedConfiguration();

        public Startup(IConfiguration configuration)
        {
            configuration.Bind(this.configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(new ConnectionFactory() { HostName = configuration.Queue.Host })
                .AddDataServices(configuration);

            services.AddGrpc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<SchedulerService>();
                endpoints.MapGrpcService<ResultsService>();
            });
        }
    }
}
