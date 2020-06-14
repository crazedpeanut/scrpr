package jk.storm.scrpr.configuration;

import org.testng.annotations.Test;

import java.util.Arrays;

import static org.fest.assertions.api.Assertions.assertThat;

public class EnvironmentVariableKeyTranslatorTest {
    @Test
    public void translateDoubleUnderscoreToPeriod() {
        String key = new EnvironmentVariableKeyTranslator().translateKey("one__two__three");

        assertThat(key).isEqualTo("one.two.three");
    }

    @Test
    public void translateColonUnderscoreToPeriod() {
        String key = new EnvironmentVariableKeyTranslator().translateKey("one:two:three");

        assertThat(key).isEqualTo("one.two.three");
    }
}