package org.apache.storm.scrpr.schemes;

import java.nio.ByteBuffer;
import java.nio.charset.StandardCharsets;
import java.util.List;

import com.google.gson.Gson;

import org.apache.storm.scrpr.models.WebScraperJob;
import org.apache.storm.tuple.Values;

public class WebScraperJobScheme extends JobScheme {

    public List<Object> deserialize(ByteBuffer ser) {
        String json = new String(ser.array(), StandardCharsets.UTF_8);
        WebScraperJob data = new Gson().fromJson(json, WebScraperJob.class);
        return new Values(data);
    }

}