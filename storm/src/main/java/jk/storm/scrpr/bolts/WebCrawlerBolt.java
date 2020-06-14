package jk.storm.scrpr.bolts;

import jk.storm.scrpr.crawlers.CrawlerController;
import jk.storm.scrpr.models.WebScraperJob;
import org.apache.storm.task.OutputCollector;
import org.apache.storm.task.TopologyContext;
import org.apache.storm.topology.BasicOutputCollector;
import org.apache.storm.topology.OutputFieldsDeclarer;
import org.apache.storm.topology.base.BaseBasicBolt;
import org.apache.storm.topology.base.BaseRichBolt;
import org.apache.storm.tuple.Tuple;

import java.util.Map;

public class WebCrawlerBolt extends BaseBasicBolt {
    @Override
    public void execute(Tuple input, BasicOutputCollector collector) {
        String target = input.getString(0);

        try
        {
            new CrawlerController(target).Begin();
        }
        catch (Exception e)
        {
            // TODO: Notify Scraper Service crawl has failed for some unknown reason
        }
    }

    @Override
    public void declareOutputFields(OutputFieldsDeclarer declarer) {}
}
