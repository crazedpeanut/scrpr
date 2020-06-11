using Scraper.Mongo;

namespace Scraper.Service.Core.Models
{
    public enum ScraperJobStatus
    {
        Pending,
        InProgress,
        Complete
    }

    public class ScraperJob : MongoEntity
    {
        public CollectorProperties Collector { get; set; }
        public string RequesterId { get; set; }
        public string SourceId { get; set; }
        public ScraperJobStatus Status { get; set; }
        public string Error { get; set; }
    }
}