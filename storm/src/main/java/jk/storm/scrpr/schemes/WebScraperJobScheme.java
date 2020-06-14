package jk.storm.scrpr.schemes;

import java.nio.ByteBuffer;
import java.nio.charset.StandardCharsets;
import java.util.List;

import com.google.gson.Gson;

import jk.storm.scrpr.models.WebScraperJob;
import org.apache.storm.spout.Scheme;
import org.apache.storm.tuple.Fields;
import org.apache.storm.tuple.Values;

public class WebScraperJobScheme implements Scheme {
    public List<Object> deserialize(ByteBuffer ser) {
        String json = new String(ser.array(), StandardCharsets.UTF_8);
        WebScraperJob job = new Gson().fromJson(json, WebScraperJob.class);
        return new Values(job.id, job.collector.target);
    }

    public Fields getOutputFields() {
        return new Fields("jobId", "target");
    }
}