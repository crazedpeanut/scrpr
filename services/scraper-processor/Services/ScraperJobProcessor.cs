using System.Threading.Tasks;

namespace ScraperProcessor.Services
{
    public class ScraperJobProcessor
    {
        private readonly ScraperJobRepository jobRespository;
        private readonly ScraperService scraperService;
        private readonly ScraperResultRespository resultRespository;
        public ScraperJobProcessor(ScraperJobRepository jobRespository, ScraperService scraperService, ScraperResultRespository resultRespository)
        {
            this.resultRespository = resultRespository;
            this.scraperService = scraperService;
            this.jobRespository = jobRespository;
        }

        public async Task Process(ScraperJob job, CancellationToken cancellationToken)
        {
            job.Status = ScraperJobStatus.Running;

            await jobRespository.Update(job, cancellationToken);

            ScraperResult result = null;

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

            if (result != null)
            {
                await resultRespository.Create(result, cancellationToken);
            }

            await jobRespository.Update(job, cancellationToken);
        }
    }
}