using SkylinkMonitoreo.Logica.Interfaces;

namespace SkylinkMonitoreo
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMonitoreoAPIS _monitoreo;
        private readonly TimeSpan _delayInterval = TimeSpan.FromMinutes(10);

        public Worker(ILogger<Worker> logger, IMonitoreoAPIS monitoreo)
        {
            _logger = logger;
            _monitoreo = monitoreo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var ultimaEjecucion = DateTime.Now.AddDays(-1);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                try
                {
                    var fechaActual = DateTime.Now;
                    // Aquí va la lógica principal del servicio
                    await this._monitoreo.MonitoreoDePlataformas(ultimaEjecucion.Date < fechaActual.Date);
                    ultimaEjecucion = fechaActual;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while monitoring platforms: {message}", ex.Message);
                }

                // Espera de 10 minutos antes de la siguiente iteración
                await Task.Delay(_delayInterval, stoppingToken);
            }
        }
    }
}
