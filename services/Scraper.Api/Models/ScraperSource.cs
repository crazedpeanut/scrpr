using GraphQL.Types;

namespace Scraper.Api.Models
{
    public class ScraperSource
    {
        public string Id { get; set; }
        public Collector Collector { get; set; }
    }

    public abstract class Collector
    {
    }

    public class WebCollector : Collector
    {
        public string Target { get; set; }
    }
}