using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

            // Directo
            channel.ExchangeDeclare("direct_exchange", ExchangeType.Direct, true);
            var queueNameDirect = channel.QueueDeclare("queue.direct", true, false, false).QueueName;
            channel.QueueBind(queueNameDirect, "direct_exchange", "direct_routing_key");

            var consumerDirect = new EventingBasicConsumer(channel);
            consumerDirect.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Recibido en Direct: '{message}'");
            };

            channel.BasicConsume(queue: queueNameDirect,
                                 autoAck: true,
                                 consumer: consumerDirect);

            // Tipo topic
            channel.ExchangeDeclare("topic_exchange", ExchangeType.Topic, true);
            var queueNameTopic1 = channel.QueueDeclare().QueueName;
            var queueNameTopic2 = channel.QueueDeclare().QueueName;
            channel.QueueBind(queueNameTopic1, "topic_exchange", "topic.routing.*");
            channel.QueueBind(queueNameTopic2, "topic_exchange", "*.routing.key");

            var consumerTopic1 = new EventingBasicConsumer(channel);
            consumerTopic1.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Recibido en Topic 1: '{message}'");
            };

            var consumerTopic2 = new EventingBasicConsumer(channel);
            consumerTopic2.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Recibido en Topic 2: '{message}'");
            };

            channel.BasicConsume(queue: queueNameTopic1,
                                 autoAck: true,
                                 consumer: consumerTopic1);

            channel.BasicConsume(queue: queueNameTopic2,
                                 autoAck: true,
                                 consumer: consumerTopic2);

            // Tipo fanout
            channel.ExchangeDeclare("fanout_exchange", ExchangeType.Fanout, true);
            var queueNameFanout1 = channel.QueueDeclare().QueueName;
            var queueNameFanout2 = channel.QueueDeclare().QueueName;
            var queueNameFanout3 = channel.QueueDeclare().QueueName;

            channel.QueueBind(queueNameFanout1, "fanout_exchange", "");
            channel.QueueBind(queueNameFanout2, "fanout_exchange", "");
            channel.QueueBind(queueNameFanout3, "fanout_exchange", "");

            var consumerFanout1 = new EventingBasicConsumer(channel);
            consumerFanout1.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Recibido en Fanout 1: '{message}'");
            };

            var consumerFanout2 = new EventingBasicConsumer(channel);
            consumerFanout2.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Recibido en Fanout 2: '{message}'");
            };

            var consumerFanout3 = new EventingBasicConsumer(channel);
            consumerFanout3.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Recibido en Fanout 3: '{message}'");
            };

            channel.BasicConsume(queue: queueNameFanout1,
                                 autoAck: true,
                                 consumer: consumerFanout1);

            channel.BasicConsume(queue: queueNameFanout2,
                                 autoAck: true,
                                 consumer: consumerFanout2);

            channel.BasicConsume(queue: queueNameFanout3,
                                 autoAck: true,
                                 consumer: consumerFanout3);

            Console.WriteLine("Presiona cualquier tecla para salir.");
            Console.ReadKey();
        }
    }
}
