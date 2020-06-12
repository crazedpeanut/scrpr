package org.apache.storm.scrpr.models;

public class ScraperJob<TProps extends CollectorProperties> {
    public String id;
    public TProps collector;
}