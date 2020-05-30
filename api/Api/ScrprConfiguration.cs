namespace Api
{
    public class ScrprConfiguration
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

    public class CollectionsConfiguration
    {
        public string ScraperResult { get; set; } = "scraper-result";
        public string ScraperJob { get; set; } = "scraper-job";
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