using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Skysense_Data.Auth.Interfaces;
using Skysense_Data.Dashboard;
using Skysense_models.BackgroundService;
using Skysense_models.Errors;
using Skysense_models.Otros;
using Skysense_persistencia.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.Auth.Implementation
{
    public class BateriasData : IBateriasData
    {
        Skysense_persistencia.Entidades.SkysenseDevContext _SkysenseDevContext;
        IConsultasComunes _InterfazData;
        IMapper _mapper;

        public BateriasData(Skysense_persistencia.Entidades.SkysenseDevContext SkysenseDevContext, IMapper mapper, IConsultasComunes interfazData) {
            _SkysenseDevContext = SkysenseDevContext;
            _mapper = mapper;
            _InterfazData = interfazData;
        }

        public async Task<bool> GuardaConfiguracionBateria(OPCBateria bateriaN)
        {
            var bess = _SkysenseDevContext.Besses.FirstOrDefault(b => b.IdInstalacion == bateriaN.IdInstalacion);
            if (bess == null)
            {
                OutputParameter<string> sNombreBDDNueva = new OutputParameter<string>();
                OutputParameter<bool?> bError = new OutputParameter<bool?>();
                OutputParameter<string> sMensaje = new OutputParameter<string>();

                await this._SkysenseDevContext.Procedures.spGeneraBDDMedicionesAsync(bateriaN.IdInstalacion, sNombreBDDNueva, bError, sMensaje);

                if (bError.Value == true)
                {
                    throw new Exception("Error al configurar la batería: " + sMensaje.Value);
                }

                bess = new Bess()
                {
                    IdInstalacion = bateriaN.IdInstalacion,
                    CadenaConexion = sNombreBDDNueva.Value,
                    Estatus = 0,
                    Monitorear = false,
                    UrlConexionBess = "",
                    UltimaActualizacionEstatus = DateTime.Now,
                };
                
                await _SkysenseDevContext.Besses.AddAsync(bess);
            }

            bess.UltimaActualizacionEstatus = DateTime.Now;
            bess.UrlConexionBess = bateriaN.Url;
            bess.Monitorear = bateriaN.Monitorear;

             await _SkysenseDevContext.SaveChangesAsync();


            foreach (var tag in bateriaN.TagsBateria)
            {
                var tagRes = await _SkysenseDevContext.TagsBesses.FirstOrDefaultAsync(tb => tb.IdTag == tag.IdTag && tb.IdBess == bateriaN.IdBateria);

                if(tagRes == null)
                {
                    tagRes = new TagsBess()
                    {
                        IdBess = bess.IdInstalacion,
                        Estatus = 0,
                        UltimaActualizacionEstatus = DateTime.Now,
                        Activo = true
                    };

                    await _SkysenseDevContext.TagsBesses.AddAsync(tagRes);
                }

                tagRes.IdParametro = (short)tag.IdParametro;
                tagRes.Monitorear = tag.Monitorear;
                tagRes.Tag = tag.Etiqueta;
                tagRes.IdGrafica = tag.IdGrafica;
                tagRes.NombreAmostrar = tag.NombreAMostrar;

                await _SkysenseDevContext.SaveChangesAsync();
            }


            //Se desactivan los tags que se borraron (eliminación lógica)
            foreach (var tag in _SkysenseDevContext.TagsBesses.Where(tb=>tb.IdBess == bateriaN.IdInstalacion)){
                if(!bateriaN.TagsBateria.Any(t=>t.IdParametro == tag.IdParametro))
                    tag.Activo = false;
            }

            await _SkysenseDevContext.SaveChangesAsync();

            return true;
        }

        public OPCBateria? ObtenConfiguracionBateria(int idInstalacion)
        {
            var bess = _SkysenseDevContext.Besses.Include(b=>b.IdInstalacionNavigation).Include(b=>b.TagsBesses).ThenInclude(t=>t.IdParametroNavigation).FirstOrDefault(b => b.IdInstalacion == idInstalacion);

            if (bess == null)
            {
                return null;
            }

            var resp = new OPCBateria()
            {
                IdBateria = bess.IdInstalacion,
                IdInstalacion = bess.IdInstalacion,
                NombreInstalacion = bess.IdInstalacionNavigation.Nombre,
                Url = bess.UrlConexionBess,
                Monitorear = bess.Monitorear,
                TagsBateria = bess.TagsBesses.Where(tb=>tb.Activo).Select(t => new TagBateria()
                {
                    Etiqueta = t.Tag,
                    IdTag = t.IdTag,
                    IdParametro = t.IdParametro,
                    Unidad = t.IdParametroNavigation.Unidad,
                    Monitorear = t.Monitorear,
                    Graficable = t.IdParametroNavigation.EsGraficable,
                    SoloLectura = t.IdParametroNavigation.SoloLectura,
                    NombreAMostrar = t.NombreAmostrar
                }).ToArray()
            };

            return resp;
        }

        public TagSelectOptions[] ObtenTiposTags()
        {
            return _SkysenseDevContext.ParametrosBesses.Select(b => new TagSelectOptions()
            {
                Label = b.Medicion + " " + b.Unidad,
                Value = b.IdParametro.ToString(),
                Graficable = b.EsGraficable,
                SoloLectura = b.SoloLectura
                
            }).ToArray();
        }

        public BateriasTabla ObtenTablaBaterias(string busqueda)
        {
            //Catálogos de Estado Monitoreo, Nivel de Alertas y Estado de alertas
            var catEstadoMonitoreo = _InterfazData.fcObtenerOpcionesCatalago(14);
            var catNivelAlerta = _InterfazData.fcObtenerOpcionesCatalago(15);
            var catEstadoAlerta = _InterfazData.fcObtenerOpcionesCatalago(16);

            return new BateriasTabla()
            {
                UltimaActualizacion = DateTime.Now,
                Baterias = _SkysenseDevContext.Besses.Include(b => b.IdInstalacionNavigation).Include(b => b.TagsBesses)
                    .Where(b => b.IdInstalacionNavigation.Nombre.Contains(busqueda))
                    .AsEnumerable() // Cambia a LINQ to Objects para permitir el uso de null propagation
                    .Select(b => new BateriaRegistro()
                    {
                        Alertas = b.BessAlerta.Count(a => a.IdEstado == 1),
                        IdEstatus = b.Estatus ?? 0,
                        Estatus = catEstadoMonitoreo.FirstOrDefault(em => em.iIdOpcion == b.Estatus)?.sOpcion ?? "",
                        IdInstalacion = b.IdInstalacion,
                        NombreInstalacion = b.IdInstalacionNavigation.Nombre,
                        MensajeUltimo = b.MensajeEstatus,
                        TagsOnline = b.TagsBesses.Count(t => t.Estatus == 2 || t.Estatus == 4),
                        TagsTotales = b.TagsBesses.Count()

                    }).ToArray()
            };
        }

    }
}
