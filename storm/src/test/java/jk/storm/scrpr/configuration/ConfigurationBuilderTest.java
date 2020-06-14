package jk.storm.scrpr.configuration;

import java.io.IOException;
import java.util.Arrays;

import org.testng.annotations.Test;

import static org.fest.assertions.api.Assertions.assertThat;

public class ConfigurationBuilderTest {

    private ConfigurationPropertyAccessor defaultAccessor = key -> {
        switch (key) {
            case "queue.host":
                return "host";
            case "queue.username":
                return "username";
            case "queue.password":
                return "password";
            case "queue.port":
                return "12345";
            case "queue.exchangeNames.scraperStart":
                return "scraperstart";
            case "queue.queueNames.webCollector":
                return "webCollector";
            default:
                return null;
        }
    };

    private ConfigurationPropertyAccessor overrideAccessor = key -> {
        switch (key) {
            case "queue.host":
                return "override";
            default:
                return null;
        }
    };

    @Test(expectedExceptions = {ConfigurationException.class })
    public void throwsIfMissingConfiguration() throws ConfigurationException {
        ConfigurationBuilder builder = new ConfigurationBuilder(key -> null);

        builder.build();
    }

    @Test
    public void buildSourceOrderDeterminesSelectedValue() throws ConfigurationException {
        ConfigurationBuilder builder = new ConfigurationBuilder(Arrays.asList(defaultAccessor, overrideAccessor));

        ScraperConfiguration config = builder.build();

        assertThat(config.queue.host).isEqualTo("override");
        assertThat(config.queue.username).isEqualTo("username");
    }
}