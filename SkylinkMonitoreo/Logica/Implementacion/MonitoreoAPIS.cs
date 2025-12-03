using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SkylinkMonitoreo.Logica.Interfaces;
using Skysense_business.APIPlataformas;
using Skysense_Data.Auth.Interfaces;
using Skysense_models.DB;
using Skysense_models.Otros;
using Skysense_models.Worker;
using Skysense_persistencia.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylinkMonitoreo.Logica.Implementacion
{
    public  class MonitoreoAPIS : IMonitoreoAPIS
    {
        private readonly IWorkerData _workerData;
        private readonly IDbContextFactory<SkysenseDevContext> _context;
        private readonly ILogger<Worker> _logger;
        public MonitoreoAPIS(IWorkerData workerData, ILogger<Worker> logger, IDbContextFactory<SkysenseDevContext> context)
        {
            _workerData = workerData;
            _logger = logger;
            _context = context;
        }

        public async Task<bool> MonitoreoDePlataformas(bool primeroDelDia)
        {

            int[] plataformas = { 3, 5, 6 }; //3: FusionSolar, 5: Solis, 6: Sungrow
            Dictionary<int, string> plataformasNombres = new Dictionary<int, string>();
            plataformasNombres.Add(3, "FusionSolar/Huawei");
            plataformasNombres.Add(5, "Solis");
            plataformasNombres.Add(6, "Sungrow");

            foreach (var plataforma in plataformas)
            {
                _logger.LogInformation($"Iniciando procesamiento de la Plataforma {plataformasNombres[plataforma]}.");

                ApiPlataforma apiPlataforma;

                try
                {
                    switch (plataforma)
                    {
                        case 3:
                            apiPlataforma = new ApiPlataformaFusionSolar();
                            break;
                        case 5:
                            apiPlataforma = new ApiPlataformaSolis();
                            break;
                        case 6:
                            apiPlataforma = new ApiPlataformaSungrow();
                            break;
                        default:
                            continue;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al iniciar configuración de la clase para la Plataforma {plataformasNombres[plataforma]}: {ex.Message}");
                    continue;
                }

                //Si es la primera ejecución del día, hacer la sincronización masiva y obtener generación del día anterior
                if (primeroDelDia)
                {
                    try
                    {
                        _logger.LogInformation($"Iniciando sincronización masiva para la Plataforma {plataformasNombres[plataforma]}");
                        await SincronizarInversoresMasivo(plataforma, apiPlataforma);
                        _logger.LogInformation($"Iniciando generación masiva para la Plataforma {plataformasNombres[plataforma]}");
                        await ObtenerGeneracionMasiva(plataforma, apiPlataforma, DateTime.Now.AddDays(-1));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error en la sincronización masiva o generación masiva para la Plataforma {plataformasNombres[plataforma]}: {ex.Message}");
                    }
                }
                else
                {
                    continue;
                    await MonitorearDiaActual(plataforma, apiPlataforma, DateTime.Now);
                }

                _logger.LogInformation($"Procesamiento de la Plataforma {plataformasNombres[plataforma]} terminado.");

            }
            return true;
        }

        private async Task MonitorearDiaActual(int plataforma, ApiPlataforma apiPlataforma, DateTime now)
        {
            throw new NotImplementedException();
        }

        private async Task ObtenerGeneracionMasiva(int plataforma, ApiPlataforma apiPlataforma, DateTime dateTime)
        {
            using var context = _context.CreateDbContext();
            var instalaciones = await _workerData.ObtenerInstalaciones(context, plataforma);
            context.Dispose();

            await this.mObtenGeneracionMasiva(apiPlataforma, dateTime, DateInterval.Month, instalaciones);
        }

        private async Task<CInversorApiencabezado[]> SincronizarInversoresMasivo(int plataforma, ApiPlataforma apiPlataforma)
        {
            using var context = _context.CreateDbContext();
            var instalaciones = await _workerData.ObtenerInstalaciones(context, plataforma);

            //Se obtienen sólamente los inversores que no están en base de datos
            var nuevosInversores = (await apiPlataforma.mObtenListaInversores(instalaciones.Select(i => i.IdAPI).ToArray()))
                .SelectMany(ins => ins.Value.Where(i => !instalaciones.SelectMany(ins => ins.InversoresAPI).Any(inv => inv.IdApi == i.identificador))
                                        .Select(i =>
                                        {
                                            var lIdInst = instalaciones.First(inst => inst.IdAPI == ins.Key).IdInstalacion;
                                            return new CInversorApiencabezado
                                            {
                                                IdApi = i.identificador,
                                                NumeroSerie = i.numeroSerie,
                                                IdInstalacion = lIdInst,
                                                IdInversorApi = 0,
                                                FechaInicio = i.fechaInicio == null ? null : DateOnly.FromDateTime(i.fechaInicio.Value),
                                                FechaFin = i.fechaFin == null ? null : DateOnly.FromDateTime(i.fechaFin.Value),
                                                IdInversor = instalaciones.SelectMany(ins => ins.InversoresAPP).FirstOrDefault(iinv => iinv.NumeroSerie == i.numeroSerie)?.IdInversor ?? 0,
                                                FechaUltimaActualizacion = DateTime.Now
                                            };
                                        }));

            return await _workerData.DarDeAltaInversoresAPI(context, nuevosInversores.ToList());
        }

        private async Task mObtenGeneracionMasiva(ApiPlataforma plataforma, DateTime fechaConsulta, DateInterval intervalo, InstalacionWorker[] instalaciones)
        {
            var generacion = await plataforma.mObtenGeneracionDiariaInversores("", fechaConsulta, instalaciones.SelectMany(i=>i.InversoresAPI).Select(i=> new InversorInfo()
            {
                identificador = i.IdApi,
                numeroSerie = i.NumeroSerie
            }).ToArray());

            if (generacion != null && generacion.Count() > 0)
            {
                //var listaGeneracion = generacion.Select(g => new CInversorApigeneracion
                //{
                //    IdInversorApi = instalaciones.SelectMany(i=>i.InversoresAPI).FirstOrDefault(i => i.IdApi == g.identificador)?.IdInversorApi ?? 0,
                //    FechaUltimaActualizacion = DateTime.Now,
                //    FechaValor = fechaConsulta,
                //    Valor = g.inversorData.FirstOrDefault(d => d.fecha.Date == fechaConsulta.Date)?.generacion ?? 0,
                //    IdConsecutivo = 0,
                //    ValorManual = false
                //}).ToList();

                var listaGeneracion = generacion.SelectMany(g => g.inversorData.Select(d => new CInversorApigeneracion()
                {
                    IdInversorApi = instalaciones.SelectMany(i => i.InversoresAPI).FirstOrDefault(i => i.IdApi == g.identificador)?.IdInversorApi ?? 0,
                    FechaUltimaActualizacion = DateTime.Now,
                    FechaValor = d.fecha.Date,
                    Valor = d.generacion,
                    IdConsecutivo = 0,
                    ValorManual = false
                }));

                using var context = _context.CreateDbContext();
                await _workerData.ActualizaGeneracionDiaria(context, listaGeneracion);
            }
        }
    }
}
