using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Scraper.Service.Data.Models;

namespace Scraper.Service.Services
{
    public class SchedulerService : Scraper.Scheduler.SchedulerBase
    {
        private readonly JobScheduler scheduler;
        public SchedulerService(JobScheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        public override async Task<ScraperJobResponse> Begin(ScraperJobRequest request, ServerCallContext context)
        {
            var job = new ScraperJob
            {
                Url = new Uri(request.Url)
            };

            var jobId = await scheduler.Begin(job);

            return new ScraperJobResponse
            {
                Id = jobId
            };
        }

        public override async Task List(Google.Protobuf.WellKnownTypes.Empty request, Grpc.Core.IServerStreamWriter<ScraperListResponse> responseStream, Grpc.Core.ServerCallContext context)
        {
            var jobs = await scheduler.List();

            foreach (var job in jobs)
            {
                await responseStream.WriteAsync(new ScraperListResponse
                {
                    Id = job.Id,
                    Url = job.Url.ToString()
                });
            }
        }

        public override Task Subscribe(Google.Protobuf.WellKnownTypes.Empty request, Grpc.Core.IServerStreamWriter<ScraperSchedulerEvent> responseStream, Grpc.Core.ServerCallContext context)
        {
            return base.Subscribe(request, responseStream, context);
        }
    }
}
