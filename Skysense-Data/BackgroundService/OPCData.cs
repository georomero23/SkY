using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skysense_models.BackgroundService;
using Skysense_models.Enums;
using Skysense_persistencia.Entidades;
using Skysense_persistencia.EntidadesBateriasMonitoreo;
using Skysense_persistencia.EntidadesBESS;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.BackgroundService
{
    public class OPCData : IOPCData
    {
        private IConfiguration _config;
        private IServiceScopeFactory _serv;
        private ILogger<OPCData> _logger;

        public OPCData(IConfiguration config, IServiceScopeFactory serv, ILogger<OPCData> logger)
        {
            _config = config;
            _serv = serv;
            _logger = logger;
        }

        public async Task<OPCBateria[]> ObtenConfiguracionesOPC(params int[] idBaterias)
        {
            using var scope = _serv.CreateScope();
            var bateriasContext = scope.ServiceProvider.GetRequiredService<SkysenseDevContext>();

            return bateriasContext.Besses.Include(b=>b.IdInstalacionNavigation).Include(b=>b.TagsBesses).ThenInclude(t=>t.IdParametroNavigation)
                .Where(b => b.Monitorear == true && 
                //Se agrega filtro para buscar baterias en específico.
                            (idBaterias.Length == 0 || idBaterias.Contains(b.IdInstalacion)))
                .Select(b => new OPCBateria()
            {
                IdBateria = b.IdInstalacion,
                IdInstalacion = b.IdInstalacion,
                Monitorear = b.Monitorear,
                NombreInstalacion = b.IdInstalacionNavigation.Nombre,
                Url = b.UrlConexionBess,
                CadenaConexion = b.CadenaConexion,
                TagsBateria = b.TagsBesses.Select(b => new TagBateria()
                {
                    Etiqueta = b.Tag,
                    Graficable = b.IdParametroNavigation.EsGraficable,
                    SoloLectura = b.IdParametroNavigation.SoloLectura,
                    IdGrafica = b.IdGrafica,
                    IdParametro = b.IdParametro,
                    IdTag = b.IdTag,
                    Monitorear = b.Monitorear,
                    NombreAMostrar = b.NombreAmostrar,
                    Unidad = b.IdParametroNavigation.Unidad
                }).ToArray()
            }).ToArray();
        }

        /// <summary>
        /// Obtiene las mediciones de las baterias.
        /// </summary>
        /// <param name="startDate">Fecha de inicio de búsqueda</param>
        /// <param name="endDate">Fecha de fin de búsqueda</param>
        /// <param name="IdBaterias">Identificadores de las baterías a buscar. No poner nada si se requiere buscar todas.</param>
        /// <returns></returns>
        public async Task<OPCBateria[]> ObtenMediciones(DateTime? startDate, DateTime? endDate, params int[] IdBaterias)
        {
            using var scope = _serv.CreateScope();
            var bateriasContext = scope.ServiceProvider.GetRequiredService<SkysenseBateriasContext>();

            return await  (from b in bateriasContext.Bess
                             where b.SiMedir == true
                             select new OPCBateria()
                             {
                                 IdBateria = b.IdBess,
                                 IdInstalacion = b.IdInstalacion,
                                 NombreInstalacion = "",
                                 Url = "",
                                 TagsBateria = b.TagsBess.Where(t => t.SiMedir == true).Select(t => new TagBateria()
                                 {
                                     IdTag = t.IdTag,
                                     Etiqueta = "",
                                     Unidad = t.IdParametroNavigation.Unidad,
                                     Mediciones = t.TagsMediciones.Where(m => startDate <= m.Fecha && m.Fecha >= endDate).Select(t => new TagMedicion()
                                     {
                                         IdTag = t.IdTag,
                                         Timestamp = t.Fecha,
                                         Medicion = t.Medicion
                                     }).ToArray()
                                 }).ToArray()
                             }).ToArrayAsync();

            //return (from t in this._bateriasContext.TagsBess
            //         join m in this._bateriasContext.TagsMediciones
            //         on t.IdTag equals m.IdTag
            //         join b in this._bateriasContext.Bess
            //         on t.IdBess equals b.IdBess
            //         where b.SiMedir == true && t.SiMedir == true && startDate <= m.Fecha && m.Fecha >= endDate && (IdBaterias.Length == 0 || IdBaterias.Contains(t.IdBess))
            //         select new OPCBateria()
            //         {
            //             IdBateria = b.IdBess,
            //             NombreInstalacion = "",
            //             IdInstalacion = b.IdInstalacion,
            //             TagsBateria = 
            //         }
        }

        public async Task GuardarMediciones(TagMedicion[] mediciones, string CadenaConexion)
        {
            try
            {
                using var scope = _serv.CreateScope();
                var bateriasContext = scope.ServiceProvider.GetRequiredService<SkysenseBateriasContext>();


                using BateriasMonitoreoContext contexto = new BateriasMonitoreoContext(Encoding.UTF8.GetString(Convert.FromBase64String(CadenaConexion)));

                await contexto.MedicionesBess.AddRangeAsync(mediciones.Select(m => new MedicionesBess()
                {
                    IdTag = m.IdTag,
                    FechaMedicion = m.Timestamp,
                    ValorMedicion = (short)m.Medicion
                }).ToArray());

                await contexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al guardar la información: {0}", ex.Message);
            }
        }

        public async Task CambiarEstado(Entidad entidad, int idEntidad, byte NuevoEstado, string? MensajeEstado)
        {
            try
            {
                using var scope = _serv.CreateScope();
                var bateriasContext = scope.ServiceProvider.GetRequiredService<SkysenseDevContext>();

                switch (entidad)
                {
                    case Entidad.Bateria:
                        var bateria = bateriasContext.Besses.Find(idEntidad);
                        if (bateria != null)
                        {
                            bateria.Estatus = NuevoEstado;
                            bateria.MensajeEstatus = MensajeEstado;
                            bateria.UltimaActualizacionEstatus = DateTime.Now;
                        }
                        else
                        {
                            _logger.LogWarning("No se encontró la batería con id {0} para cambiar el estado.", idEntidad);
                        }
                        break;
                    case Entidad.Tag:
                        var tag = bateriasContext.TagsBesses.Find(idEntidad);
                        if (tag != null)
                        {
                            tag.Estatus = NuevoEstado;
                            tag.MensajeEstatus = MensajeEstado;
                            tag.UltimaActualizacionEstatus = DateTime.Now;
                        }
                        else
                        {
                            _logger.LogWarning("No se encontró el tag con id {0} para cambiar el estado.", idEntidad);
                        }
                        break;
                    default:
                        throw new ArgumentException("Entidad no especificada para cambiar estado.");
                }
                await bateriasContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al cambiar el estado: {0}", ex.Message);
            }
        }
    }
}
