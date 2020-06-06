using System;
using System.Threading;
using System.Threading.Tasks;
using Scraper.Service.Data.Models;
using Scraper.Service.Data.Services;
using Google.Protobuf;

namespace Scraper.Service.Processor.Services
{
    public class ScraperJobProcessor
    {
        private readonly ScraperJobRepository jobRespository;
        private readonly ScraperFactory scraperFactory;
        private readonly ScraperResultRepository resultRespository;
        private readonly JobNotificationBroadcaster notificationBroadcaster;

        public ScraperJobProcessor(
            ScraperJobRepository jobRespository,
            ScraperFactory scraperFactory,
            ScraperResultRepository resultRespository,
            JobNotificationBroadcaster notificationBroadcaster)
        {
            this.resultRespository = resultRespository;
            this.notificationBroadcaster = notificationBroadcaster;
            this.jobRespository = jobRespository;
            this.scraperFactory = scraperFactory;
        }

        public async Task Process(ScraperJobMessage message, CancellationToken cancellationToken)
        {
            var job = await jobRespository.Get(message.Id);
            job.Status = Data.Models.ScraperJobStatus.Running;

            await jobRespository.Update(job);
            notificationBroadcaster.Emit(job);

            ScraperResult result = null;

            try
            {
                result = await scraperFactory.GetScraper(job.Url).Scrape(cancellationToken);
            }
            catch (Exception)
            {
                job.Error = "Unable to scrape page";
            }
            finally
            {
                job.Status = Data.Models.ScraperJobStatus.Complete;
            }

            if (result != null)
            {
                await resultRespository.Create(result);
            }

            await jobRespository.Update(job);
            notificationBroadcaster.Emit(job);
        }
    }
}