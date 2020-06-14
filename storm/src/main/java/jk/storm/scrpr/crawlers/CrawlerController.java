package jk.storm.scrpr.crawlers;

import com.google.common.io.Files;
import edu.uci.ics.crawler4j.crawler.CrawlConfig;
import edu.uci.ics.crawler4j.crawler.CrawlController;
import edu.uci.ics.crawler4j.fetcher.PageFetcher;
import edu.uci.ics.crawler4j.robotstxt.RobotstxtConfig;
import edu.uci.ics.crawler4j.robotstxt.RobotstxtServer;

public class CrawlerController {
    private static final int CONCURRENT_PAGES = 8;
    private static final int CONCURRENT_CRAWLERS = 8;
    private String target;

    public CrawlerController(String target)
    {
        this.target = target;
    }

    public void Begin() throws Exception
    {
        String crawlStorageFolder = Files.createTempDir().getAbsolutePath();

        CrawlConfig config = new CrawlConfig();

        config.setPolitenessDelay(100);

        config.setCrawlStorageFolder(crawlStorageFolder);

        config.setMaxPagesToFetch(CONCURRENT_PAGES);

        /*
         * Instantiate the controller for this crawl.
         */
        PageFetcher pageFetcher = new PageFetcher(config);
        RobotstxtConfig robotstxtConfig = new RobotstxtConfig();
        RobotstxtServer robotstxtServer = new RobotstxtServer(robotstxtConfig, pageFetcher);
        CrawlController controller = new CrawlController(config, pageFetcher, robotstxtServer);

        /*
         * For each crawl, you need to add some seed urls. These are the first
         * URLs that are fetched and then the crawler starts following links
         * which are found in these pages
         */
        controller.addSeed(target);
        /*
         * Start the crawl. This is a blocking operation, meaning that your code
         * will reach the line after this only when crawling is finished.
         */
        controller.start(new CrawlerFactory(), CONCURRENT_CRAWLERS);
    }
}
