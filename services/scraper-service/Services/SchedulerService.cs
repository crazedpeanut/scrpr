using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Scraper;
using ScraperService.Models;

namespace ScraperService.Services
{
    public class SchedulerService : Scraper.Scheduler.SchedulerBase
    {
        private readonly ScraperJobRepository jobRepository;
        public SchedulerService(ScraperJobRepository jobRepository)
        {
            this.jobRepository = jobRepository;
        }

        public override async Task<ScraperJobResponse> Begin(ScraperJobRequest request, ServerCallContext context)
        {
            var job = new ScraperJob
            {
                Url = new Uri(request.Url)
            };

            await jobRepository.Create(job);

            return new ScraperJobResponse
            {
                Id = job.Id,
                Url = job.Url.ToString()
            };
        }

        public override Task List(Google.Protobuf.WellKnownTypes.Empty request, Grpc.Core.IServerStreamWriter<ScraperJob> responseStream, Grpc.Core.ServerCallContext context)
        {
            return base.List(request, responseStream, context);
        }

        public override Task Subscribe(Google.Protobuf.WellKnownTypes.Empty request, Grpc.Core.IServerStreamWriter<ScraperSchedulerEvent> responseStream, Grpc.Core.ServerCallContext context)
        {
            return base.Subscribe(request, responseStream, context);
        }
    }
}
