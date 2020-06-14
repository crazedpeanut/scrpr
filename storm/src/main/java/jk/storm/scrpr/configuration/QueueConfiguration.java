package jk.storm.scrpr.configuration;

public class QueueConfiguration {
    public String host;
    public String username;
    public String password;
    public int port;
    public QueueNames queueNames = new QueueNames();
    public ExchangeNames exchangeNames = new ExchangeNames();
}
