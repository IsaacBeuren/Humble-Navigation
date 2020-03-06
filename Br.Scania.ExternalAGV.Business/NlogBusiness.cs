using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using NLog.Extensions.Logging;
using NLog;

namespace Br.Scania.ExternalAGV.Business
{
    public class NlogBusiness
    {
        static IConfigurationRoot config;
        static IServiceProvider servicesProvider;
        static Logger logger;
        static Runner runner;

        public NlogBusiness()
        {
            config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .Build();

            servicesProvider = BuildDi(config);

            logger = LogManager.GetCurrentClassLogger();

            runner = servicesProvider.GetRequiredService<Runner>();
        }

        private static IServiceProvider BuildDi(IConfigurationRoot config)
        {
            return new ServiceCollection()
               .AddTransient<Runner>() // Runner is the custom class
               .AddLogging(loggingBuilder =>
               {
                   // configure Logging with NLog
                   loggingBuilder.ClearProviders();
                   loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                   loggingBuilder.AddNLog(config);
               })
               .BuildServiceProvider();
        }

        public void Write(string message)
        {
//#if (DEBUG)
            using (servicesProvider as IDisposable)
            {
                runner.DoAction(message);
            }
//#endif
        }

        public void Error(Exception ex, string message)
        {
            using (servicesProvider as IDisposable)
            {
                logger.Error(ex, message);
            }
        }

        public class Runner
        {
            private readonly ILogger<Runner> _logger;

            public Runner(ILogger<Runner> logger)
            {
                _logger = logger;
            }

            public void DoAction(string name)
            {
                _logger.LogDebug(20, "{Action}", name);
            }
        }

        public void Shutdown()
        {
            LogManager.Shutdown();
        }
    }
}
