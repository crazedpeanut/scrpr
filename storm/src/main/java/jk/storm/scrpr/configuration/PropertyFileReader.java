package jk.storm.scrpr.configuration;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

public class PropertyFileReader implements ConfigurationPropertyAccessor {

    private Properties properties;

    public PropertyFileReader(Properties properties)
    {
        this.properties = properties;
    }

    public static ConfigurationPropertyAccessor read(String fileName) throws IOException {
        Properties prop = new Properties();

        InputStream inputStream = PropertyFileReader.class.getClassLoader().getResourceAsStream(fileName);

        if (inputStream != null) {
            prop.load(inputStream);
            return new PropertyFileReader(prop);
        } else {
            throw new FileNotFoundException(String.format("Property file {0} not found", fileName));
        }
    }

    @Override
    public String get(String key) {
        return properties.getProperty(translateKey(key));
    }

    private String translateKey(String key)
    {
        return key.toLowerCase();
    }
}

