namespace Scraper.Service.Core.Models
{
    public class ScraperCompleteMessage
    {
        public string JobId { get; set; }
        public string CollectorKind { get; set; }
        public object Results { get; set; }
    }
}