using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
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

        public override async Task<ScraperListResponse> List(Empty request, ServerCallContext context)
        {
            var results = await scheduler.List();

            var response = new ScraperListResponse();
            response.Results.AddRange(results.Select(r => new ScraperListResponseItem
            {
                Id = r.Id,
                Url = r.Url.ToString()
            }));

            return response;
        }

        public override Task Subscribe(Empty request, IServerStreamWriter<ScraperSchedulerEvent> responseStream, ServerCallContext context)
        {
            return base.Subscribe(request, responseStream, context);
        }
    }
}
