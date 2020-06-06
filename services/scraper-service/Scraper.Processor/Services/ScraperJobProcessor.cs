using System;
using System.Threading;
using System.Threading.Tasks;
using Scraper.Data.Models;
using Scraper.Data.Services;

namespace Scraper.Processor.Services
{
    public class ScraperJobProcessor
    {
        private readonly ScraperJobRepository jobRespository;
        private readonly ScraperFactory scraperFactory;
        private readonly ScraperResultRepository resultRespository;
        public ScraperJobProcessor(
            ScraperJobRepository jobRespository, 
            ScraperFactory scraperFactory, 
            ScraperResultRepository resultRespository)
        {
            this.resultRespository = resultRespository;
            this.jobRespository = jobRespository;
            this.scraperFactory = scraperFactory;
        }

        public async Task Process(ScraperJobMessage message, CancellationToken cancellationToken)
        {
            var job = await jobRespository.Get(message.Id);
            job.Status = ScraperJobStatus.Running;

            await jobRespository.Update(job);

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
                job.Status = ScraperJobStatus.Complete;
            }

            if (result != null)
            {
                await resultRespository.Create(result);
            }

            await jobRespository.Update(job);
        }
    }
}