namespace Scraper.Service.Core.Models
{
    public class ScraperStartMessage
    {
        public string JobId { get; set; }
        public CollectorProperties Collector { get; set; }
    }
}