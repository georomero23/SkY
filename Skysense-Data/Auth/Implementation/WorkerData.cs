using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Skysense_Data.Auth.Interfaces;
using Skysense_models.DB;
using Skysense_models.Worker;
using Skysense_persistencia.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.Auth.Implementation
{
    public class WorkerData : IWorkerData
    {
        //private readonly IDbContextFactory<SkysenseDevContext> __context;
        private readonly IMapper _mapper;

        public WorkerData(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<bool> ActualizaGeneracionDiaria(SkysenseDevContext _context, IEnumerable<CInversorApigeneracion> generacionInversores)
        {
            var fechaActual = DateTime.Now;

            //Por cada inversor, se actualiza la generacion
            foreach (var invG in generacionInversores.GroupBy(g=> g.IdInversorApi))
            {
                var invEDB = _context.InversorApiencabezados.SingleOrDefault(i => i.IdInversorApi == invG.Key);

                if (invEDB != null)
                {
                    invEDB.FechaUltimaActualizacion = fechaActual;

                    foreach (var invg in invG)
                    {
                        var invgDB = _context.InversorApigeneracions
                            .FirstOrDefault(i => i.IdInversorApi == invg.IdInversorApi && DateOnly.FromDateTime(i.FechaValor) == DateOnly.FromDateTime(invg.FechaValor));

                        //Si ya existia un registro, se actualiza (si no es valor manual)
                        if (invgDB != null)
                        {
                            // Si el valor no es manual, se actualiza
                            invgDB.Valor = invg.Valor;
                            //invgDB.FechaValor = invg.FechaValor;
                            invgDB.FechaUltimaActualizacion = invg.FechaUltimaActualizacion;
                        }
                        //Si no existia, se crea uno nuevo
                        else
                        {
                            invgDB = new InversorApigeneracion()
                            {
                                IdInversorApi = invg.IdInversorApi,
                                IdConsecutivo = (_context.InversorApigeneracions.Where(i=>i.IdInversorApi == invg.IdInversorApi).Max(i => (int?)i.IdConsecutivo)??0) + 1,
                                Valor = invg.Valor,
                                FechaValor = invg.FechaValor.Date,
                                FechaUltimaActualizacion = fechaActual,
                                ValorManual = false
                            };

                            await _context.InversorApigeneracions.AddAsync(invgDB);
                        }

                        await _context.SaveChangesAsync();

                    }
                    //Se actualiza la fecha de ultima actualizacion del daemon api en la instalacion
                    _context.Instalaciones.Single(i => i.IdInstalacion == invEDB.IdInstalacion).FechaUltimaActualizacionDaemonApi = fechaActual;
                    await _context.SaveChangesAsync();
                }

            }

            return true;
        }

        public async Task<CInversorApiencabezado[]> DarDeAltaInversoresAPI(SkysenseDevContext _context, List<CInversorApiencabezado> inversores)
        {
            foreach (var inv in inversores)
            {
                var invDB = _context.InversorApiencabezados.SingleOrDefault(i => i.IdInversorApi == inv.IdInversorApi);
                if (invDB == null)
                {
                    var invNuevo = new InversorApiencabezado()
                    {
                        IdInstalacion = inv.IdInstalacion,
                        IdInversor = inv.IdInversor == 0? null: inv.IdInversor,
                        FechaInicio = inv.FechaInicio??_context.Instalaciones.FirstOrDefault(i=>i.IdInstalacion == inv.IdInstalacion)?.InicioOperaciones?? DateOnly.FromDateTime(DateTime.Now.AddYears(-5)),
                        FechaFin = inv.FechaFin,
                        FechaUltimaActualizacion = inv.FechaUltimaActualizacion,
                        IdApi = inv.IdApi
                    };

                    await _context.InversorApiencabezados.AddAsync(invNuevo);

                    await _context.SaveChangesAsync();

                    //Actualizar el IdInversorApi en el objeto original
                    inv.IdInversorApi = invNuevo.IdInversorApi;
                }
            }

            return inversores.ToArray();
        }

        public async Task mActualizaGeneracionMensualParametros(SkysenseDevContext context, int idInstalacion, int month, int year)
        {
            var mensual = context.InstalacionApigeneracionMensuals.FirstOrDefault(i=> i.IdInstalacion == idInstalacion && i.Mes == month && i.Anno == year);
            if (mensual != null)
            {
                mensual.DaemonYaEjecutado = true;
            }
            else
            {
                mensual = new InstalacionApigeneracionMensual()
                {
                    IdInstalacion = idInstalacion,
                    Mes = (byte)month,
                    Anno = (short)year,
                    DaemonYaEjecutado = true,
                    GeneracionGarantizada = null
                };
                await context.InstalacionApigeneracionMensuals.AddAsync(mensual);
            }
            await context.SaveChangesAsync();
            return;
        }

        public async Task<InstalacionWorker[]> ObtenerInstalaciones(SkysenseDevContext _context, int idPlataforma)
        {
            var instDB = await _context.Instalaciones
                .Where(i => i.IdPlataforma == idPlataforma && !string.IsNullOrEmpty(i.IdInstalacionApi))
                .ToArrayAsync();

            return instDB.Select(i => new InstalacionWorker
            {
                IdInstalacion = i.IdInstalacion,
                IdAPI = i.IdInstalacionApi!,
                IdPlataforma = idPlataforma,
                Nombre = i.Nombre??"",
                FechaUltimaActualizacionDaemonAPI = i.FechaUltimaActualizacionDaemonApi,
                InversoresAPP = this._mapper.Map<CInversore[]>(_context.Inversors.Where(inv => inv.IdInstalacion == i.IdInstalacion)).ToList(),
                InversoresAPI = this._mapper.Map<CInversorApiencabezado[]>(_context.InversorApiencabezados.Where(inv => inv.IdInstalacion == i.IdInstalacion)).ToList(),
                FechaInicio = i.InicioOperaciones,
            }).ToArray();
        }

        public async Task<InstalacionWorker> ObtenerInstalacion(SkysenseDevContext _context, int idInstalacion)
        {
            var instDB = await _context.Instalaciones
                .FirstOrDefaultAsync(i => i.IdInstalacion== idInstalacion);

            return new InstalacionWorker()
            {
                IdInstalacion = instDB.IdInstalacion,
                IdAPI = instDB.IdInstalacionApi!,
                IdPlataforma = instDB.IdPlataforma??throw new ArgumentException("¡La instalación no tiene Plataforma configurada!"),
                Nombre = instDB.Nombre ?? "",
                FechaUltimaActualizacionDaemonAPI = instDB.FechaUltimaActualizacionDaemonApi,
                InversoresAPP = this._mapper.Map<CInversore[]>(_context.Inversors.Where(inv => inv.IdInstalacion == idInstalacion)).ToList(),
                InversoresAPI = this._mapper.Map<CInversorApiencabezado[]>(_context.InversorApiencabezados.Where(inv => inv.IdInstalacion == idInstalacion)).ToList(),
                FechaInicio = instDB.InicioOperaciones
            };
        }
    }
}
