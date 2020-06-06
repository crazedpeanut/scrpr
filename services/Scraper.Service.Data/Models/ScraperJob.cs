using System;

namespace Scraper.Service.Data.Models
{
    public class ScraperJob
    {
        public ScraperJob() { }
        public ScraperJob(string id, Uri url, ScraperJobStatus status, string error)
        {
            this.Id = id;
            this.Url = url;
            this.Status = status;
            this.Error = error;
        }

        public string Id { get; set; }
        public Uri Url { get; set; }
        public ScraperJobStatus Status { get; set; }
        public string Error { get; set; }
    }

    public enum ScraperJobStatus
    {
        Created = 0,
        Running = 1,
        Complete = 2
    }
}