using System;
using System.IO;
using System.Threading;

namespace Br.Scania.ExternalAGV.DockerCron
{
    class Program
    {
        static void Main(string[] args)
        {

            Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            try
            {
                File.AppendAllText("log", $"I'm alive: {DateTime.Now}{Environment.NewLine}");
                File.WriteAllText("healtcheck", "1");
            }
            catch (Exception)
            {
                File.WriteAllText("healtcheck", "0");
                Environment.ExitCode = 1;
            }

        }
    }
}
