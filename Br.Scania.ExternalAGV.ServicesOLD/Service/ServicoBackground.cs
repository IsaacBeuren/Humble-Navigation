using Br.Scania.ExternalAGV.Business;
using Br.Scania.ExternalAGV.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Br.Scania.ExternalAGV.Services.BackgroundService
{
    // https://imasters.com.br/dotnet/tarefas-de-background-com-servicos-hospedados-no-asp-net-core
    public class ServicoBackground : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        TruBusiness tru;

        public ServicoBackground(ILogger<ServicoBackground> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço de backgorund Iniciado");
            tru = new TruBusiness("COM6");
            _timer = new Timer(ExecutarTarefa, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5));

            return Task.CompletedTask;

        }

        private void ExecutarTarefa(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");
            MemoryBusiness memory = new MemoryBusiness();
            while (true)
            {
                NMEAModel nmea = tru.ReadGPS();

                if (nmea != null)
                {
                    memory.List2Write(nmea);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço de backgorund Finalizado");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

    }
}
