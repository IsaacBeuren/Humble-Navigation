using System;
using Br.Scania.ExternalAGV.Model;
using Br.Scania.ExternalAGV.Business;
using System.Threading;
using Topshelf;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using Br.Scania.ExternalAGV.Model.DataBase;

namespace Br.Scania.ExternalAGV.ReadNmea
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

        static bool portOK = false;
        static ConfigModel config;
        static int SerialPort;
        static RabbitMQBusiness rabbitMQBusiness;

        static void Main(string[] args)
        {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            rabbitMQBusiness = new RabbitMQBusiness();


            IConfigurationRoot configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("Storage");

            int AGVID = Convert.ToInt16(configuration.GetSection("AGV").GetSection("ID").Value);
            string Token = configuration.GetSection("Token").GetSection("ID").Value;
            SerialPort = Convert.ToInt16(configuration.GetSection("GPS").GetSection("Serial").Value);

            LogFileBusiness log = new LogFileBusiness();
            try
            {
                ConfigBusiness configBusiness = new ConfigBusiness();
                config = configBusiness.GetById(AGVID);

                SerialPortBusiness serialPort = new SerialPortBusiness();

                portOK = serialPort.CheckSerialPort("COM" + SerialPort);

                if (portOK)
                {
                    HostFactory.Run(configurator =>
                    {
                        configurator.Service<TaskAntenna>(s =>
                        {
                            s.ConstructUsing(name => new TaskAntenna());
                            s.WhenStarted((service, controlAntena) => service.Start(controlAntena));
                            s.WhenStopped((service, controlAntena) => service.Stop(controlAntena));
                        });

                        configurator.RunAsLocalSystem();
                        configurator.SetDescription("Thread para consulta do sinal GPS");
                    });
                }
                else
                {
                    log.Write("Porta COM" + SerialPort + " não existe!");
                    Console.WriteLine("Porta COM" + SerialPort + " não existe!");
                }

                Console.WriteLine("Tecle para Sair!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
            }
        }


        public sealed class TaskAntenna : ServiceControl
        {
            LogFileBusiness log = new LogFileBusiness();
            ConnectionFactoryModel connectionFactory = new ConnectionFactoryModel();

            private readonly CancellationTokenSource _cancellationTokenSource;

            public TaskAntenna()
            {
                _cancellationTokenSource = new CancellationTokenSource();
            }

            public bool Start(HostControl hostControl)
            {
                TextFileBusiness textFile = new TextFileBusiness();
                TruBusiness tru = new TruBusiness(SerialPort);
                var queueName = Guid.NewGuid().ToString();
                Task.Run(() =>
                {
                    while (true)
                    {
                        try
                        {
                            NMEAModel nmea = tru.ReadGPS();
                            if (nmea != null)
                            {
                                //textFile.WriteText(JsonConvert.SerializeObject(nmea));
                                rabbitMQBusiness.PublishMessage("Nmea", JsonConvert.SerializeObject(nmea));
                            }
                        }
                        catch (Exception)
                        {

                        }
                        Thread.Sleep(TimeSpan.FromSeconds(0.5));
                    }
                }, _cancellationTokenSource.Token);
                return true;
            }

            public bool Stop(HostControl hostControl)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                return true;
            }

        }

    }
}




























