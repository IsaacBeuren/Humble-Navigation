using Br.Scania.ExternalAGV.Business;
using Br.Scania.ExternalAGV.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Br.Scania.ExternalAGV.PLC
{
    class Program
    {
        static void Main()
        {
            PlcBusiness plc = new PlcBusiness();

            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "scania",
                Password = "Scania2019"
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    try
                    {
                        channel.QueueDeclare(queue: "Commands2PLC",
                                                durable: true,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);

                        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                        Console.WriteLine(" [*] Waiting for messages.");

                        var consumer = new EventingBasicConsumer(channel);
                        bool ret = false;
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Commands2PLCModel commands = JsonConvert.DeserializeObject<Commands2PLCModel>(message);
                            int dots = message.Split('.').Length - 1;
                            Thread.Sleep(dots * 500);
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                            // Write PLC 

                            ret = plc.WriteCommandsPLC(commands);

                            if (ret)
                            {
                                channel.BasicConsume(queue: "Commands2PLC",
                                     autoAck: false,
                                     consumer: consumer);
                                ret = false;
                            }
                        };
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Erro escrita PLC");
                    }

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}