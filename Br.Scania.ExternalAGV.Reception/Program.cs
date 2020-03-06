using Br.Scania.ExternalAGV.Business;
using Br.Scania.ExternalAGV.Model;
using Br.Scania.ExternalAGV.Model.DataBase;
using CoordinateSharp;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Br.Scania.ExternalAGV.Reception
{
    class Program
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        static Coordinate last_but_one;
        static bool Stopped;
        static bool MoreThanOne;
        //static PointsModel pointsModel=null;

        static void Main()
        {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

            int AGVID = Convert.ToInt16(1);

            CoordinatesBusiness coordinatesBusiness = new CoordinatesBusiness();
            PointsBusiness points = new PointsBusiness();
            PlcBusiness plc = new PlcBusiness();
            utilBusiness util = new utilBusiness();
            NmeaBusiness nmea = new NmeaBusiness();

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
                        PointsBusiness pointsBusiness = new PointsBusiness();
                        PointsModel nextPoint = pointsBusiness.GetNextToExecute(AGVID,2);
                        LastPositionBusiness lastPositionBusiness = new LastPositionBusiness();

                        channel.ExchangeDeclare(exchange: "Nmea", type: "fanout");

                        var queueName = channel.QueueDeclare().QueueName;
                        channel.QueueBind(queue: queueName,
                                          exchange: "Nmea",
                                          routingKey: "");

                        Console.WriteLine(" [*] Waiting for logs.");

                        Console.WriteLine("Buscar Ponto " + nextPoint.Description);

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            NMEAModel NMEAReceived = JsonConvert.DeserializeObject<NMEAModel>(message);
                            GGAModel ggaReceived = NMEAReceived.GGAModel;
                            FilterBusiness filter = new FilterBusiness();
                            GGAModel gga = filter.ApplyFilter(ggaReceived);

                            if (gga != null)
                            {
                                DateTime utcDate = DateTime.UtcNow;
                                LastPositionModel lastPosition = new LastPositionModel();
                                lastPosition.ID = AGVID;
                                lastPosition.Latitude = Convert.ToDouble(gga.Latitude);
                                lastPosition.Longitude = Convert.ToDouble(gga.Longitude);
                                lastPosition.UpdateTime = utcDate;
                                lastPosition.GPSQuality = Convert.ToInt32(gga.GPS_Quality_indicator);
                                lastPositionBusiness.Update(lastPosition);


                                //Actual GPS Position 
                                Coordinate coordinateActual = new Coordinate();
                                coordinateActual = coordinatesBusiness.DegreeDecimalMinute2Coordinate(gga.Latitude, gga.Longitude);

                                //Last GPS Position 
                                Coordinate coordinateLastest = new Coordinate();
                                coordinateLastest = last_but_one;

                                Distance course = coordinatesBusiness.Calculating_Distance(coordinateActual, coordinateLastest);

                                last_but_one = coordinateActual;

                                if (nextPoint != null && course != null)
                                {
                                    Coordinate coordinateDestination = new Coordinate();
                                    coordinateDestination = coordinatesBusiness.DegreeDecimalMinute2Coordinate(nextPoint.Lat, nextPoint.Lng);
                                    Distance distance2Destination = coordinatesBusiness.Calculating_Distance(coordinateDestination, coordinateActual);
                                    Commands2PLCModel commands = new Commands2PLCModel();
                                    double angle = coordinatesBusiness.AngularCalculation(course.Bearing, distance2Destination.Bearing);

                                    try
                                    {
                                        if (distance2Destination.Meters <= 1.5)
                                        {
                                            if (points.PostPointDone(nextPoint))
                                            {
                                                nextPoint = points.GetNextToExecute(AGVID,2);
                                                Console.WriteLine("Buscando Ponto " + nextPoint.Description);
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {


                                    }

                                    if (nextPoint != null)
                                    {
                                        commands.StopAuto = false;
                                    }
                                    else
                                    {
                                        commands.StopAuto = true;
                                    }

                                    if (course != null)
                                    {
                                        //Console.WriteLine(course.Meters);

                                        if (course.Meters >= 0.0015)
                                        {
                                            //Console.WriteLine(course.Meters);
                                            if (MoreThanOne)
                                            {
                                                Stopped = false;
                                                //Console.Write("DISTANCIA - " + course.Meters);
                                                //Console.Write(" // CURSO - " + course.Bearing);
                                                Console.Write("GPS Quality - " + nmea.GetGPSQualityIndicator(gga.GPS_Quality_indicator));
                                                Console.Write(" // Satelites - " + gga.Number_of_SVs_in_use);
                                                Console.Write("  // DISTANCIA - " + distance2Destination.Meters);
                                                Console.WriteLine(" // DIFERENÇA - " + angle);

                                                // Aplica tratamento entre Angulos
                                                commands.Degree = Convert.ToInt32(angle * 100);
                                            }
                                            MoreThanOne = true;
                                        }
                                        else
                                        {
                                            if (!Stopped)
                                            {
                                                Console.WriteLine("Veiculo Parado");
                                            }
                                            commands.Degree = 0;
                                            Stopped = true;
                                        }
                                    }
                                    bool ret = plc.WriteCommandsPLC(commands);
                                }
                            }
                        };
                        channel.BasicConsume(queue: queueName,
                                             autoAck: true,
                                             consumer: consumer);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Erro recepção GGA");
                    }

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}


//using Br.Scania.ExternalAGV.Business;
//using Br.Scania.ExternalAGV.Model;
//using CoordinateSharp;
//using Microsoft.Extensions.Configuration;
//using Newtonsoft.Json;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System;
//using System.IO;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading;

//namespace Br.Scania.ExternalAGV.Reception
//{
//    class Program
//    {
//        private const int MF_BYCOMMAND = 0x00000000;
//        public const int SC_CLOSE = 0xF060;

//        [DllImport("user32.dll")]
//        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

//        [DllImport("user32.dll")]
//        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

//        [DllImport("kernel32.dll", ExactSpelling = true)]
//        private static extern IntPtr GetConsoleWindow();

//        static Coordinate last_but_one;
//        static bool Stopped;
//        static bool MoreThanOne;
//        //static PointsModel pointsModel=null;

//        static void Main()
//        {
//            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

//            int AGVID = Convert.ToInt16(1);

//            CoordinatesBusiness coordinatesBusiness = new CoordinatesBusiness();
//            PointsBusiness points = new PointsBusiness();
//            PlcBusiness plc = new PlcBusiness();
//            utilBusiness util = new utilBusiness();

//            ConnectionFactory factory = new ConnectionFactory()
//            {
//                HostName = "localhost",
//                UserName = "scania",
//                Password = "Scania2019"
//            };

//            using (var connection = factory.CreateConnection())
//            {
//                using (var channel = connection.CreateModel())
//                {
//                    try
//                    {
//                        channel.QueueDeclare(queue: "ZDA",
//                             durable: true,
//                             exclusive: false,
//                             autoDelete: false,
//                             arguments: null);

//                        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

//                        //channel.QueueDelete("ZDA", false, false);

//                        Console.WriteLine(" [*] Waiting for messages.");

//                        var consumerZDA = new EventingBasicConsumer(channel);
//                        consumerZDA.Received += (model, ea) =>
//                        {
//                            var body = ea.Body;
//                            var message = Encoding.UTF8.GetString(body);
//                            ZDAModel zda = JsonConvert.DeserializeObject<ZDAModel>(message);
//                            int dots = message.Split('.').Length - 1;
//                            Thread.Sleep(100);
//                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
//                        };
//                        channel.BasicConsume(queue: "ZDA",
//                                             autoAck: false,
//                                             consumer: consumerZDA);
//                    }
//                    catch (Exception)
//                    {
//                        Console.WriteLine("Erro recepção ZDA");
//                    }

//                    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

//                    try
//                    {
//                        channel.QueueDeclare(queue: "GST",
//                             durable: true,
//                             exclusive: false,
//                             autoDelete: false,
//                             arguments: null);

//                        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

//                        //channel.QueueDelete("GST", false, false);

//                        Console.WriteLine(" [*] Waiting for messages.");

//                        var consumerGST = new EventingBasicConsumer(channel);
//                        consumerGST.Received += (model, ea) =>
//                        {
//                            var body = ea.Body;
//                            var message = Encoding.UTF8.GetString(body);
//                            GSTModel gst = JsonConvert.DeserializeObject<GSTModel>(message);
//                            int dots = message.Split('.').Length - 1;
//                            Thread.Sleep(100);
//                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
//                        };
//                        channel.BasicConsume(queue: "GST",
//                                             autoAck: false,
//                                             consumer: consumerGST);
//                    }
//                    catch (Exception)
//                    {
//                        Console.WriteLine("Erro recepção GST");
//                    }

//                    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

//                    try
//                    {
//                        PointsBusiness pointsBusiness = new PointsBusiness();
//                        PointsModel nextPoint = pointsBusiness.GetNextToExecute(AGVID);

//                        Console.WriteLine("Buscar Ponto " + nextPoint.Description);

//                        channel.QueueDeclare(queue: "GGA",
//                             durable: true,
//                             exclusive: false,
//                             autoDelete: false,
//                             arguments: null);

//                        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

//                        //channel.QueueDelete("GGA", false, false);

//                        Console.WriteLine(" [*] Waiting for messages.");

//                        var consumerGGA = new EventingBasicConsumer(channel);
//                        consumerGGA.Received += (model, ea) =>
//                        {
//                            var body = ea.Body;
//                            var message = Encoding.UTF8.GetString(body);
//                            GGAModel ggaReceived = JsonConvert.DeserializeObject<GGAModel>(message);

//                            FilterBusiness filter = new FilterBusiness();
//                            GGAModel gga = filter.ApplyFilter(ggaReceived);

//                            //Actual GPS Position 
//                            Coordinate coordinateActual = new Coordinate();
//                            coordinateActual = coordinatesBusiness.DegreeDecimalMinute2Coordinate(gga.Latitude, gga.Longitude);

//                            //Last GPS Position 
//                            Coordinate coordinateLastest = new Coordinate();
//                            coordinateLastest = last_but_one;

//                            Distance course = coordinatesBusiness.Calculating_Distance(coordinateActual, coordinateLastest);

//                            last_but_one = coordinateActual;

//                            if (nextPoint != null && course != null)
//                            {
//                                Coordinate coordinateDestination = new Coordinate();
//                                coordinateDestination = coordinatesBusiness.DegreeDecimalMinute2Coordinate(nextPoint.Lat, nextPoint.Lng);

//                                Distance distance2Destination = coordinatesBusiness.Calculating_Distance(coordinateDestination, coordinateActual);

//                                Commands2PLCModel commands = new Commands2PLCModel();

//                                double angle = coordinatesBusiness.AngularCalculation(course.Bearing, distance2Destination.Bearing);

//                                try
//                                {
//                                    if (distance2Destination.Meters <= 1.5)
//                                    {
//                                        if (points.PostPointDone(nextPoint))
//                                        {
//                                            nextPoint = points.GetNextToExecute(AGVID);
//                                            Console.WriteLine("Buscando Ponto " + nextPoint.Description);
//                                        }
//                                    }
//                                }
//                                catch (Exception)
//                                {


//                                }

//                                if (nextPoint != null)
//                                {
//                                    commands.StopAuto = false;
//                                }
//                                else
//                                {
//                                    commands.StopAuto = true;
//                                }

//                                if (course != null)
//                                {
//                                    //Console.WriteLine(course.Meters);

//                                    if (course.Meters >= 0.0015)
//                                    {
//                                        Console.WriteLine(course.Meters);
//                                        if (MoreThanOne)
//                                        {
//                                            Stopped = false;
//                                            //Console.Write("DISTANCIA - " + course.Meters);
//                                            //Console.Write(" // CURSO - " + course.Bearing);
//                                            NmeaBusiness nmea = new NmeaBusiness();
//                                            Console.Write("GPS Quality - " + nmea.GetGPSQualityIndicator(gga.GPS_Quality_indicator));

//                                            Console.Write(" // Satelites - " + gga.Number_of_SVs_in_use);
//                                            Console.Write("  // DISTANCIA - " + distance2Destination.Meters);
//                                            Console.WriteLine(" // DIFERENÇA - " + angle);

//                                            // Aplica tratamento entre Angulos
//                                            commands.Degree = Convert.ToInt32(angle * 100);
//                                        }
//                                        MoreThanOne = true;
//                                    }
//                                    else
//                                    {
//                                        if (!Stopped)
//                                        {
//                                            Console.WriteLine("Veiculo Parado");
//                                        }
//                                        commands.Degree = 0;
//                                        Stopped = true;
//                                    }
//                                }
//                                bool ret = plc.WriteCommandsPLC(commands);
//                            }

//                            int dots = message.Split('.').Length - 1;
//                            Thread.Sleep(100);

//                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
//                        };
//                        channel.BasicConsume(queue: "GGA",
//                                                autoAck: false,
//                                                consumer: consumerGGA);
//                    }
//                    catch (Exception)
//                    {
//                        Console.WriteLine("Erro recepção GGA");
//                    }



//                    Console.WriteLine(" Press [enter] to exit.");
//                    Console.ReadLine();
//                }
//            }
//        }
//    }
//}