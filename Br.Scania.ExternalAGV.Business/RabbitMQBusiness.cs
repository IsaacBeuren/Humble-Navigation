using Br.Scania.ExternalAGV.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Br.Scania.ExternalAGV.Business
{
    public class RabbitMQBusiness
    {
        ConnectionFactory factory;

        public RabbitMQBusiness(ConnectionFactory connectionFactory)
        {
            factory = new ConnectionFactory()
            {
                HostName = connectionFactory.HostName,
                UserName = connectionFactory.UserName,
                Password = connectionFactory.Password
            };
        }

        public string Read(string queue)
        {
            try
            {

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queue,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    Console.WriteLine(" [*] Waiting for messages.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: queue,
                                         autoAck: false,
                                         consumer: consumer);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
                throw;
            }

        }

        public void WriteQueue(string queue, string message)
        {
            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queue,
                                            durable: true,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                            routingKey: queue,
                                            basicProperties: properties,
                                            body: Encoding.UTF8.GetBytes(message));
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Erro");
            }
        }

        public void PublishMessage(string queue, string messageToSend)
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: queue, type: "fanout");

                var message = messageToSend;
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: queue,
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);
            }
        }

        //public NMEAModel Subscribe()
        //{
        //    ConnectionFactory factory = new ConnectionFactory()
        //    {
        //        HostName = "localhost",
        //        UserName = "scania",
        //        Password = "Scania2019"
        //    };

        //    using (var connection = factory.CreateConnection())
        //    {
        //        using (var channel = connection.CreateModel())
        //        {
        //            try
        //            {
        //                channel.ExchangeDeclare(exchange: "Nmea", type: "fanout");

        //                var queueName = channel.QueueDeclare().QueueName;
        //                channel.QueueBind(queue: queueName,
        //                                  exchange: "Nmea",
        //                                  routingKey: "");

        //                var consumer = new EventingBasicConsumer(channel);
        //                consumer.Received += (model, ea) =>
        //                {
        //                    var body = ea.Body;
        //                    var message = Encoding.UTF8.GetString(body);
        //                    NMEAModel NMEAReceived = JsonConvert.DeserializeObject<NMEAModel>(message);
        //                    return NMEAReceived;
        //                };
        //                channel.BasicConsume(queue: queueName,
        //                                     autoAck: true,
        //                                     consumer: consumer);
        //            }
        //            catch (Exception ex)
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //    return null;
        //}
    }
}
