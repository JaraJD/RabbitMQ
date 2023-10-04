using System;
using RabbitMQ.Client;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Exchange directo
            channel.ExchangeDeclare("direct_exchange", ExchangeType.Direct, true);
            channel.QueueDeclare("queue.direct", true, false ,false);
            channel.QueueBind("queue.direct", "direct_exchange", "direct_routing_key");
            var message = "Mensaje para el exchange directo";
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("direct_exchange", "direct_routing_key", null, body);
            Console.WriteLine($"Enviado: '{message}'");

            // Exchange topic
            channel.ExchangeDeclare("topic_exchange", ExchangeType.Topic, true);
            channel.QueueDeclare("queue.topic1", true, false, false);
            channel.QueueDeclare("queue.topic2", true, false, false);
            channel.QueueBind("queue.topic1", "topic_exchange", "topic.routing.key1");
            channel.QueueBind("queue.topic2", "topic_exchange", "topic.routing.queue.key2");
            var message1 = "Mensaje para el exchange de tipo topic1";
            var message2 = "Mensaje para el exchange de tipo topic2";
            var message3 = "Mensaje para ambas colas que estan conectadas al exchange de tipo topic";
            var body1 = Encoding.UTF8.GetBytes(message1);
            var body2 = Encoding.UTF8.GetBytes(message2);
            var body3 = Encoding.UTF8.GetBytes(message3);
            channel.BasicPublish("topic_exchange", "topic.routing.key1", null, body1);
            channel.BasicPublish("topic_exchange", "topic.routing.queue.key2", null, body2);
            channel.BasicPublish("topic_exchange", "topic.routing.*", null, body3);
            Console.WriteLine($"Enviado: '{message1}'");
            Console.WriteLine($"Enviado: '{message2}'");
            Console.WriteLine($"Enviado: '{message3}'");

            //Exchange fanout
            channel.ExchangeDeclare("fanout_exchange", ExchangeType.Fanout, true);
            channel.QueueDeclare("queue.fanout1", true, false, false);
            channel.QueueDeclare("queue.fanout2", true, false, false);
            channel.QueueDeclare("queue.fanout3", true, false, false);
            channel.QueueBind("queue.fanout1", "fanout_exchange", "");
            channel.QueueBind("queue.fanout2", "fanout_exchange", "");
            channel.QueueBind("queue.fanout3", "fanout_exchange", "");
            var message4 = "Mensaje para el exchange de tipo fanout";
            var body4 = Encoding.UTF8.GetBytes(message4);
            channel.BasicPublish("fanout_exchange", "", null, body4);
            Console.WriteLine($"Enviado: '{message4}'");
        }
    }
}
