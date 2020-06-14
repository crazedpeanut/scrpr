package jk.storm.scrpr.crawlers;

import edu.uci.ics.crawler4j.crawler.CrawlController;

public class CrawlerFactory implements CrawlController.WebCrawlerFactory<SimpleWebCrawler> {
    public SimpleWebCrawler newInstance() throws Exception {
        return new SimpleWebCrawler();
    }
}