using System;

namespace shared
{
    public class SharedConfiguration
    {
        public DatabaseConfiguration Database { get; set; } = new DatabaseConfiguration();
        public QueueConfiguration Queue { get; set; } = new QueueConfiguration();
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

    public class CacheConfiguration
    {
        public CacheItemsConfiguration Items { get; set; } = new CacheItemsConfiguration();
    }


    public class CacheItemsConfiguration
    {
        public CacheItemConfiguration ScraperResults = new CacheItemConfiguration
        {
            BaseKey = "/scraper-results"
        };

        public CacheItemConfiguration ScraperJobs = new CacheItemConfiguration
        {
            BaseKey = "/scraper-jobs",
            TTL = 86400
        };
    }


    public class CacheItemConfiguration
    {
        public string BaseKey { get; set; }

        //Seconds
        public int? TTL { get; set; }
    }

    public class CollectionConfiguration
    {
        public string Name { get; set; }
    }

    public class CollectionsConfiguration
    {
        public CollectionConfiguration Sample = new CollectionConfiguration
        {
            Name = "sample"
        };
    }

    public class QueueConfiguration
    {
        public string Host { get; set; }
        public QueueNames QueueNames { get; set; } = new QueueNames();
    }

    public class QueueNames
    {
        public string ScraperStart { get; set; } = "scraper-start";
    }
}
