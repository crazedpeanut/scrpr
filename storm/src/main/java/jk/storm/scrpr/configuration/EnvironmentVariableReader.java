package jk.storm.scrpr.configuration;

public class EnvironmentVariableReader implements ConfigurationPropertyAccessor
{
    public String prefix;
    private KeyTranslator keyTranslator = new EnvironmentVariableKeyTranslator();

    public EnvironmentVariableReader(String prefix)
    {
        this.prefix = prefix;
    }

    @Override
    public String get(String key) {
        return System.getenv(keyTranslator.translateKey(prefix + key));
    }}
