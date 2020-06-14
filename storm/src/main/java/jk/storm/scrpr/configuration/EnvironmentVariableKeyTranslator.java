package jk.storm.scrpr.configuration;

public class EnvironmentVariableKeyTranslator extends KeyTranslator {

    @Override
    public String translateKey(String key)
    {
        return super.translateKey(
            key
                .replace("__", ".")
                .replace(":", "."));
    }
}
