package org.apache.storm.scrpr.topologies;

import com.rabbitmq.client.ConnectionFactory;

import org.apache.storm.StormSubmitter;
import org.apache.storm.generated.AlreadyAliveException;
import org.apache.storm.generated.AuthorizationException;
import org.apache.storm.generated.InvalidTopologyException;
import org.apache.storm.generated.StormTopology;
import org.apache.storm.scrpr.schemes.WebScraperJobScheme;
import org.apache.storm.spout.Scheme;
import org.apache.storm.topology.IRichSpout;
import org.apache.storm.topology.TopologyBuilder;
import io.latent.storm.rabbitmq.RabbitMQSpout;
import io.latent.storm.rabbitmq.config.ConnectionConfig;
import io.latent.storm.rabbitmq.config.ConsumerConfig;
import io.latent.storm.rabbitmq.config.ConsumerConfigBuilder;

/**
 * A Trident topology example.
 */
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
            throws AlreadyAliveException, InvalidTopologyException, AuthorizationException {

        Scheme scheme = new WebScraperJobScheme();
        IRichSpout spout = new RabbitMQSpout(scheme);

        TopologyBuilder builder = new TopologyBuilder();

        ConnectionConfig connectionConfig = new ConnectionConfig("localhost", 5672, "guest", "guest",
                ConnectionFactory.DEFAULT_VHOST, 10);

        ConsumerConfig spoutConfig = new ConsumerConfigBuilder().connection(connectionConfig)
                .queue("your.rabbitmq.queue").prefetch(200).requeueOnFail().build();

        builder.setSpout("my-spout", spout).addConfigurations(spoutConfig.asMap()).setMaxSpoutPending(200);

        StormTopology topology = builder.createTopology();

        StormSubmitter.submitTopology(TOPOLOGY_NAME, null, topology);
    }

    /**
     * Utility constructor to prevent initialization.
     */
    private WebCollectorTopology() {
    }
}
