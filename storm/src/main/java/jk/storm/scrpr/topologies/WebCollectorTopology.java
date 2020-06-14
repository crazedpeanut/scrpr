package jk.storm.scrpr.topologies;

import java.io.IOException;
import java.util.HashMap;

import com.rabbitmq.client.ConnectionFactory;

import io.latent.storm.rabbitmq.Declarator;
import io.latent.storm.rabbitmq.RabbitMQSpout;
import io.latent.storm.rabbitmq.config.ConnectionConfig;
import io.latent.storm.rabbitmq.config.ConsumerConfig;
import io.latent.storm.rabbitmq.config.ConsumerConfigBuilder;

import jk.storm.scrpr.bolts.WebCrawlerBolt;
import jk.storm.scrpr.configuration.ConfigurationException;
import org.apache.storm.StormSubmitter;
import org.apache.storm.generated.AlreadyAliveException;
import org.apache.storm.generated.AuthorizationException;
import org.apache.storm.generated.InvalidTopologyException;
import org.apache.storm.generated.StormTopology;
import org.apache.storm.spout.Scheme;
import org.apache.storm.topology.IRichSpout;
import org.apache.storm.topology.TopologyBuilder;


import jk.storm.scrpr.RabbitMQDeclarator;
import jk.storm.scrpr.configuration.ConfigurationBuilder;
import jk.storm.scrpr.configuration.ScraperConfiguration;
import jk.storm.scrpr.schemes.WebScraperJobScheme;
import org.apache.storm.tuple.Fields;


public final class WebCollectorTopology {
    private static final String TOPOLOGY_NAME = "web-collector-topology";

    /**
     * The example's main method.
     *
     * @param args the command line arguments
     * @throws AlreadyAliveException    if the topology is already started
     * @throws InvalidTopologyException if the topology is invalid
     * @throws AuthorizationException   if the topology authorization fails
     */
    public static void main(final String[] args)
            throws AlreadyAliveException, InvalidTopologyException, AuthorizationException, IOException, ConfigurationException {

        ScraperConfiguration config = ConfigurationBuilder.Create().build();

        Declarator queueDeclaration = new RabbitMQDeclarator(
                config.queue.exchangeNames.scraperStart,
                config.queue.queueNames.webCollector,
                "web.*");

        Scheme scheme = new WebScraperJobScheme();
        IRichSpout spout = new RabbitMQSpout(scheme, queueDeclaration);

        TopologyBuilder builder = new TopologyBuilder();

        ConnectionConfig connectionConfig = new ConnectionConfig(
                config.queue.host,
                config.queue.port,
                config.queue.username,
                config.queue.password,
                ConnectionFactory.DEFAULT_VHOST,
                10);

        ConsumerConfig spoutConfig = new ConsumerConfigBuilder()
                .connection(connectionConfig)
                .queue(config.queue.queueNames.webCollector)
                .prefetch(200)
                .requeueOnFail()
                .build();

        builder.setSpout("web-collector", spout)
                .addConfigurations(spoutConfig.asMap())
                .setMaxSpoutPending(200);

        builder.setBolt("web-crawler", new WebCrawlerBolt())
                .fieldsGrouping("web-collector", new Fields("jobId", "target"));

        StormTopology topology = builder.createTopology();

        StormSubmitter.submitTopology(TOPOLOGY_NAME, new HashMap<>(), topology);
    }

    /**
     * Utility constructor to prevent initialization.
     */
    private WebCollectorTopology() {
    }
}

