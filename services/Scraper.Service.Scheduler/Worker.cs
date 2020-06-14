using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Scraper.Service.Scheduler
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public Worker(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var scope = serviceProvider.CreateScope();
                await scope.ServiceProvider.GetRequiredService<JobScheduler>().Process();
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
