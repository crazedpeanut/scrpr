package jk.storm.scrpr.configuration;

import java.io.IOException;

import java.util.*;
import java.util.stream.Collectors;

public class ConfigurationBuilder {
    List<ConfigurationPropertyAccessor> sources;

    public ConfigurationBuilder(List<ConfigurationPropertyAccessor> sources) {
        this.sources = sources;
    }

    public ConfigurationBuilder(ConfigurationPropertyAccessor source) {
        this.sources = new ArrayList<>();
        this.sources.add(source);
    }

    public static ConfigurationBuilder Create() throws IOException {
        List<ConfigurationPropertyAccessor> sources = new ArrayList<>();
        sources.add(PropertyFileReader.read("application.properties"));
        sources.add(new EnvironmentVariableReader("SCRPR_"));
        return new ConfigurationBuilder(sources);
    }

    public ScraperConfiguration build() throws ConfigurationException {

        QueueConfiguration queue = new QueueConfiguration();
        queue.host = get("queue.host");
        queue.username = get("queue.username");
        queue.password = get("queue.password");
        queue.port = Integer.parseInt(get("queue.port"));
        queue.exchangeNames = new ExchangeNames();
        queue.exchangeNames.scraperStart = get("queue.exchangeNames.scraperStart");
        queue.queueNames = new QueueNames();
        queue.queueNames.webCollector = get("queue.queueNames.webCollector");

        ScraperConfiguration scraperConfiguration = new ScraperConfiguration();
        scraperConfiguration.queue = queue;

        return scraperConfiguration;
    }

    private String get(String key) throws ConfigurationException {
        List<String> values = this.sources
                .stream()
                .map(accessor -> accessor.get(key))
                .filter(value -> value != null)
                .collect(Collectors.toList());

        if (values.size() == 0) {
            throw new ConfigurationException("Missing expected item " + key);
        }

        return values.get(values.size() - 1);
    }
}


