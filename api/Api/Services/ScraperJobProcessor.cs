using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Models;

namespace Api.Services
{
    public class ScraperJobProcessor
    {
        private readonly ScraperJobRepository jobRespository;
        private readonly ScraperService scraperService;
        public ScraperJobProcessor(ScraperJobRepository jobRespository, ScraperService scraperService)
        {
            this.scraperService = scraperService;
            this.jobRespository = jobRespository;
        }

        public async Task Process(ScraperJob job, CancellationToken cancellationToken)
        {
            job.Status = ScraperJobStatus.Running;

            await jobRespository.Update(job, cancellationToken);

            ScraperResult result;

            try
            {
                result = await scraperService.ScrapeUrl(job.Url, cancellationToken);
            }
            catch (Exception)
            {
                job.Error = "Unable to scrape page";
            }
            finally
            {
                job.Status = ScraperJobStatus.Complete;
            }

            await jobRespository.Update(job, cancellationToken);
        }
    }
}