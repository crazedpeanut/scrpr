package jk.storm.scrpr.configuration;

import org.testng.annotations.Test;

import static org.fest.assertions.api.Assertions.assertThat;

public class KeyTranslatorTest {
    @Test
    public void translateKeyToLowerCase() {
        String key = new KeyTranslator().translateKey("One.Two.Three");

        assertThat(key).isEqualTo("one.two.three");
    }
}