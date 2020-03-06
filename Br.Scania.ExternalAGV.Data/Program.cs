using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Br.Scania.ExternalAGV.Data
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() + "\\Properties")
                .AddJsonFile("launchSettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("Storage");

            Console.WriteLine("Hello World!");
        }
    }
}
