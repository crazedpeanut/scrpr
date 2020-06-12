using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Scraper.Mongo;
using Scraper.Service.Core.Models;
using Scraper.Service.Core.Services;

namespace Scraper.Service.Scheduler
{
    public class JobScheduler
    {
        private readonly IRepository<ScraperJob> jobRepository;
        private readonly IRepository<ScraperSource> sourceRepository;
        private readonly ScraperJobPublisher jobPublisher;
        private readonly ILogger<JobScheduler> logger;

        public JobScheduler(IRepository<ScraperJob> jobRepository, IRepository<ScraperSource> sourceRepository, ScraperJobPublisher jobPublisher, ILogger<JobScheduler> logger)
        {
            this.jobPublisher = jobPublisher;
            this.logger = logger;
            this.sourceRepository = sourceRepository;
            this.jobRepository = jobRepository;
        }

        // FIXME: This pulls the entire collection of sources and then for each source, it checks if it has an active job
        // This needs to be optimised
        public async Task Process()
        {
            var sources = await sourceRepository.Query(_ => true);

            logger.LogInformation($"Checking {sources.Count} sources for scheduling");

            foreach (var source in sources)
            {
                var inProgressJobs = await jobRepository.Query(_ => source.Id == _.SourceId && (_.Status == ScraperJobStatus.InProgress || _.Status == ScraperJobStatus.Pending));
                if (inProgressJobs.Count == 0)
                {
                    await jobPublisher.Publish(new ScraperJob
                    {
                        SourceId = source.Id,
                        Collector = source.Collector,
                        OwnerServiceId = source.OwnerServiceId
                    });
                }
            }
        }
    }
}