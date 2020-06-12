package org.apache.storm.scrpr.schemes;

import java.nio.ByteBuffer;
import java.util.List;

import org.apache.storm.spout.Scheme;
import org.apache.storm.tuple.Fields;

public abstract class JobScheme implements Scheme {
    public static final String JOB_SCHEME_KEY = "job";

    public abstract List<Object> deserialize(ByteBuffer ser);

    public Fields getOutputFields() {
        return new Fields(JOB_SCHEME_KEY);
    }

}