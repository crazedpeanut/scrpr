using System;

namespace Scraper.Configuration
{
    public class ScraperConfiguration
    {
        public DatabaseConfiguration Database { get; set; } = new DatabaseConfiguration();
        public QueueConfiguration Queue { get; set; } = new QueueConfiguration();
        public ServicesConfiguration Services { get; set; } = new ServicesConfiguration();
    }

    public class ServiceConfiguration
    {
        public string BaseUrl { get; set; }
        public string Certificate { get; set; }
    }

    public class ServicesConfiguration
    {
        public ServiceConfiguration Scraper { get; set; } = new ServiceConfiguration();
    }

    public class DatabaseConfiguration
    {
        public string Name { get; set; } = "scrpr";
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseTls { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CollectionsConfiguration Collections { get; set; } = new CollectionsConfiguration();
    }

    public class CollectionConfiguration
    {
        public string Name { get; set; }
    }

    public class CollectionsConfiguration
    {
        public CollectionConfiguration ScraperSources = new CollectionConfiguration
        {
            Name = "scraper-source"
        };

        public CollectionConfiguration ScraperJobs = new CollectionConfiguration
        {
            Name = "scraper-job"
        };
    }

    public class QueueConfiguration
    {
        public string Host { get; set; }
        public QueueNames QueueNames { get; set; } = new QueueNames();
        public ExchangeNames ExchangeNames { get; set; } = new ExchangeNames();
    }

    public class ExchangeNames
    {
        public string ScraperStart { get; set; } = "scraper-start";
    }

    public class QueueNames
    {
    }
}
