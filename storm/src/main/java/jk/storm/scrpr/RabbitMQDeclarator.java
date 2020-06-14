package jk.storm.scrpr;

import com.rabbitmq.client.Channel;
import io.latent.storm.rabbitmq.Declarator;

import java.io.IOException;
import java.util.HashMap;

public class RabbitMQDeclarator implements Declarator {
        private final String exchange;
        private final String queue;
        private final String routingKey;

        public RabbitMQDeclarator(String exchange, String queue) {
          this(exchange, queue, "");
        }

        public RabbitMQDeclarator(String exchange, String queue, String routingKey) {
          this.exchange = exchange;
          this.queue = queue;
          this.routingKey = routingKey;
        }

        @Override
        public void execute(Channel channel) {
          try {
            channel.queueDeclare(queue, true, false, false, new HashMap<>());
            channel.exchangeDeclare(exchange, "topic", true);
            channel.queueBind(queue, exchange, routingKey);
          } catch (IOException e) {
            throw new RuntimeException("Error executing rabbitmq declarations.", e);
          }
        }
      }
