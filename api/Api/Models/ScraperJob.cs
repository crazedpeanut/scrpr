using System;

namespace Api.Models
{
    public class ScraperJob: MongoEntity
    {
        public ScraperJob() { }
        public ScraperJob(string id, Uri url, ScraperJobStatus status, string error): base(id)
        {
            this.Url = url;
            this.Status = status;
            this.Error = error;
        }

        public Uri Url { get; set; }
        public ScraperJobStatus Status { get; set; }
        public string Error { get; set; }
    }

    public enum ScraperJobStatus
    {
        Created,
        Running,
        Complete
    }
}