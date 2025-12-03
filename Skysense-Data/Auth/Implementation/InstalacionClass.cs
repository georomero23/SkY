using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Skysense_Data.Auth.Interfaces;
using Skysense_models.DB;
using Skysense_models.DB.FunctionReturns;
using Skysense_models.Errors;
using Skysense_models.Otros;
using Skysense_models.Respuestas;
using Skysense_models.Worker;
using Skysense_persistencia.Entidades;
using Skysense_persistencia.FunctionModels;
using System;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Skysense_Data.Auth.Implementation
{
    public class InstalacionClass : IInsatalaciones
    {
        Skysense_persistencia.Entidades.SkysenseDevContext _SkysenseDevContext;
        Skysense_Data.Dashboard.IConsultasComunes _consultasComunes;

        IMapper _mapper;

        public InstalacionClass(Skysense_persistencia.Entidades.SkysenseDevContext SkysenseDevContext,
            Skysense_Data.Dashboard.IConsultasComunes consultasComunes, IWorkerData worker ,IMapper mapper)
        {
            this._SkysenseDevContext = SkysenseDevContext;
            this._consultasComunes = consultasComunes;
            this._mapper = mapper;
        }

        public CInstalacione? mActualizaInstalacion(CInstalacione cInstalacion)
        {
            var instalacion = _SkysenseDevContext.Instalaciones.Where(i => i.IdInstalacion == cInstalacion.IdInstalacion && i.IdCliente == cInstalacion.IdCliente).SingleOrDefault();
            if (instalacion != null)
            {
                instalacion.Ubicacion = cInstalacion.Ubicacion;
                instalacion.IdCatalogoEstatus = cInstalacion.IdCatalogoEstatus;
                instalacion.IdPlataforma = cInstalacion.IdPlataforma;
                instalacion.CodigoProyecto = cInstalacion.CodigoProyecto;
                instalacion.Estado = cInstalacion.Estado;
                instalacion.GarantiaFin = cInstalacion.GarantiaFin;
                instalacion.GarantiaInicio = cInstalacion.GarantiaInicio;
                instalacion.IdCatalogoGarantiaEstatus = cInstalacion.IdCatalogoGarantiaEstatus;
                instalacion.IdCatalogoTarifa = cInstalacion.IdCatalogoTarifa;
                instalacion.IdCliente = cInstalacion.IdCliente;
                instalacion.IdInstalacion = cInstalacion.IdInstalacion;
                instalacion.Latitud = cInstalacion.Latitud;
                instalacion.Longitud = cInstalacion.Longitud;
                instalacion.Nombre = cInstalacion.Nombre;
                instalacion.IdGrupo = cInstalacion.IdGrupo == 0? null : cInstalacion.IdGrupo;
                instalacion.Rpu = cInstalacion.Rpu;
                instalacion.ContactoEmail = cInstalacion.ContactoEmail;
                instalacion.ContactoNombre = cInstalacion.ContactoNombre;
                instalacion.ContactoTelefono = cInstalacion.ContactoTelefono;
                instalacion.PotenciaInstalada = cInstalacion.PotenciaInstalada;
                instalacion.EsFinanciado = cInstalacion.EsFinanciado;
                instalacion.InicioOperaciones = cInstalacion.InicioOperaciones;
                instalacion.IdInstalacionApi = cInstalacion.IdInstalacionApi;
                instalacion.AnioInstalacion = cInstalacion.AnioInstalacion;
                instalacion.Zona = cInstalacion.Zona;
                instalacion.TipoProyecto = cInstalacion.TipoProyecto;


                _SkysenseDevContext.SaveChanges();

                return this.mObtenInstalacionCompleta(instalacion.IdInstalacion);
            }

            return null;
        }

        public bool mEliminaInstalacion(CInstalacione cInstalacion)
        {

            //var instalacion = _SkysenseDevContext.Instalacions.Where(c => c.IdInstalacion == cInstalacion.IdInstalacion).SingleOrDefault();
            //if (instalacion != null)
            //{
            //    //instalacion.Estado = 0;
            //    //Que hacer en estos casos?

            //    _SkysenseDevContext.SaveChanges();

            //    return true;
            //}

            return false;
        }

        public CInstalacione? mObtenInstalacion(int idCliente, int idInstalacion)
        {
            var catalogoMarcaPaneles = this._consultasComunes.fcObtenerOpcionesCatalago(4);
            var catalogoMarcaInversores = this._consultasComunes.fcObtenerOpcionesCatalago(5);

            var res = this._mapper.Map<CInstalacione>((from i in _SkysenseDevContext.Instalaciones
                                                       where i.IdCliente == idCliente &&
                                                       i.IdInstalacion == idInstalacion
                                                       select i)
                                                       //.Include(i=>i.Inversores)
                                                       .Include(i => i.Paneles)
                                                       .FirstOrDefault());

            if (res != null)
            {
            res.sTarifa = this._consultasComunes.fcObtenerOpcionesCatalago(1)
                .FirstOrDefault(c => c.iIdOpcion == res.IdCatalogoTarifa)?.sOpcion ?? "";

            res.sEstatus = this._consultasComunes.fcObtenerOpcionesCatalago(2)
                .FirstOrDefault(c => c.iIdOpcion == res.IdCatalogoTarifa)?.sOpcion ?? "";
            }

            return res;
        }

        public Paginacion<CInstalacione> mObtenInstalacionesPaginadas(int? idGrupo, string? busqueda, int paginaPaginacion, int registrosPorPagina)
        {
            var catalogoMarcaPaneles = this._consultasComunes.fcObtenerOpcionesCatalago(4);
            var catalogoMarcaInversores = this._consultasComunes.fcObtenerOpcionesCatalago(5);
            var catalogoTarifas = this._consultasComunes.fcObtenerOpcionesCatalago(1);
            var catalogoEstatus = this._consultasComunes.fcObtenerOpcionesCatalago(2);

            //busqueda por nombre o cliente
            var res = this._mapper.Map<CInstalacione[]>((from i in _SkysenseDevContext.Instalaciones
                                                         where (idGrupo < 0 || (i.IdGrupo ?? 0) == idGrupo) &&
                                                         (busqueda == null ||
                                                         (i.Nombre != null && i.Nombre.Contains(busqueda)) ||
                                                         (i.IdClienteNavigation != null && i.IdClienteNavigation.Rfc != null && i.IdClienteNavigation.Rfc.Contains(busqueda)) ||
                                                         (i.Rpu != null && i.Rpu.Contains(busqueda))||
                                                         (i.CodigoProyecto != null && i.CodigoProyecto.Contains(busqueda))
                                                         )
                                                         select i).Include(i => i.IdGrupoNavigation).ToArray());
            foreach (var ins in res)
            {
                ins.sTarifa = catalogoTarifas.FirstOrDefault(c => c.iIdOpcion == ins.IdCatalogoTarifa)?
                    .sOpcion ?? "";

                ins.sEstatus = catalogoEstatus.FirstOrDefault(c => c.iIdOpcion == ins.IdCatalogoEstatus)?
                    .sOpcion ?? "";

                foreach (var p in ins.Paneles)
                {
                    //p.sMarcaPanel = catalogoMarcaPaneles
                    //    .FirstOrDefault(m => m.iIdOpcion == p.IdCatalogoMarcaPanel)?
                    //    .sOpcion ?? "Sin marca";
                }
            }

            return new Paginacion<CInstalacione>()
            {
                paginaActual = paginaPaginacion,
                masDatos = res.Length == registrosPorPagina + 1,
                numeroPaginas = (int)Math.Ceiling((res.Length / (decimal)registrosPorPagina)),
                totalRegistros = res.Count(),
                registrosPorPagina = registrosPorPagina,
                registros = res.Skip(registrosPorPagina * (paginaPaginacion - 1)).Take(registrosPorPagina).ToArray()
            };
        }

        public CInstalacione[] mObtenInstalaciones(int IdCliente, int? IdInstalacion = null)
        {
            return _mapper.Map<CInstalacione[]>(this._SkysenseDevContext.Instalaciones.Where(i => i.IdCliente == IdCliente && (IdInstalacion == null || i.IdInstalacion == IdInstalacion))
                //.Include(i => i.Paneles)
                //.Include(i => i.Inversores)
                //.Include(i => i.IdGrupoNavigation)
                .ToList());
        }

        public CInstalacione mNuevaInstalacion(CInstalacione cInstalacion)
        {
            var nuevaInstalacion = new Skysense_persistencia.Entidades.Instalacione()
            {
                Ubicacion = cInstalacion.Ubicacion,
                IdCatalogoEstatus = cInstalacion.IdCatalogoEstatus,
                IdPlataforma = cInstalacion.IdPlataforma,
                Zona = cInstalacion.Zona,
                CodigoProyecto = cInstalacion.CodigoProyecto,
                Estado = cInstalacion.Estado,
                GarantiaFin = cInstalacion.GarantiaFin,
                GarantiaInicio = cInstalacion.GarantiaInicio,
                IdCatalogoGarantiaEstatus = cInstalacion.IdCatalogoGarantiaEstatus,
                IdCatalogoTarifa = cInstalacion.IdCatalogoTarifa,
                IdCliente = cInstalacion.IdCliente,
                IdInstalacion = (_SkysenseDevContext.Instalaciones.Max(i=> (int?)i.IdInstalacion) ?? 0 )+1,
                Latitud = cInstalacion.Latitud,
                Longitud = cInstalacion.Longitud,
                Nombre = cInstalacion.Nombre,
                IdGrupo = cInstalacion.IdGrupo == 0? null : cInstalacion.IdGrupo,
                Rpu = cInstalacion.Rpu,
                ContactoEmail = cInstalacion.ContactoEmail,
                ContactoNombre = cInstalacion.ContactoNombre,
                ContactoTelefono = cInstalacion.ContactoTelefono,
                PotenciaInstalada = cInstalacion.PotenciaInstalada,
                EsFinanciado = cInstalacion.EsFinanciado,
                InicioOperaciones = cInstalacion.InicioOperaciones,
                IdInstalacionApi = cInstalacion.IdInstalacionApi,
                AnioInstalacion = cInstalacion.AnioInstalacion
            };

            _SkysenseDevContext.Instalaciones.Add(nuevaInstalacion);
            _SkysenseDevContext.SaveChanges();

            return this.mObtenInstalacionCompleta(nuevaInstalacion.IdInstalacion);
        }

        async Task<IEnumerable<CDatosInversoresAjustado>> IInsatalaciones.mObtenDataInversoresAjustada(int idInstalacion, int iAnion)
        {
            CDatosInversoresAjustado[] datos = [];

            return this._mapper.Map<IEnumerable<CDatosInversoresAjustado>>(datos);
        }

        async Task<IEnumerable<CDatosInversoresAjustado>> IInsatalaciones.mObtenDataInversoresAjustada(int idInstalacion, int iAnion, int iMes)
        {
            CDatosInversoresAjustado[] datos = [];

            return this._mapper.Map<IEnumerable<CDatosInversoresAjustado>>(datos);
        }

        public async Task<int> mInsertaNuevosPaneles(int idInstalacion, IEnumerable<CPanele> paneles)
        {
            if (paneles.Count() == 0) return 0;

            var panelList = paneles.ToList();

            for (int i = 0; i < panelList.Count(); i++)
            {
                for (int j = i + 1; j < panelList.Count(); j++)
                {
                    if (panelList[i].Modelo == panelList[j].Modelo && panelList[i].NumeroSerie == panelList[j].NumeroSerie)
                    {
                        throw new ErrorAlCliente("Se están intentando introducir paneles con modelo y número de serie iguales.");
                    }
                }

                if(_SkysenseDevContext.Paneles.Any(p=> p.Modelo == panelList[i].Modelo && p.NumeroSerie == panelList[i].NumeroSerie))  
                {
                    throw new ErrorAlCliente("Se está intentando guardar un registro con modelo y número de serie ya existente.");
                }
            }

            //var maxId = _SkysenseDevContext.Paneles.Where(p => p.IdInstalacion == paneles.First().IdInstalacion).Max(p => (int?)p.IdPanel) ?? 0;

            await _SkysenseDevContext.Paneles.AddRangeAsync(paneles.Select(p => new Panele()
            {
                //IdPanel = ++maxId,
                DegradacionAnual = p.DegradacionAnual,
                IdEstadoPanel = p.IdEstadoPanel,
                IdInstalacion = idInstalacion,
                Modelo = p.Modelo,
                Marca = p.Marca,
                ProveedorIntermediario = p.ProveedorIntermediario,
                ProveedorIntermediarioRfc = p.ProveedorIntermediarioRfc,
                ProveedorSuministrador = p.ProveedorSuministrador,
                ProveedorSuministradorRfc = p.ProveedorSuministradorRfc,
                EsInicial = p.EsInicial,
                NumeroSerie = p.NumeroSerie,
                NumeroSerieReemplazo = p.NumeroSerieReemplazo,
                Danado = p.Danado,
                Potencia = p.Potencia
            }));

            return await _SkysenseDevContext.SaveChangesAsync();
        }

        public CPanele[] mObtenPaneles(int idInstalacion)
        {
            return this._mapper.Map<CPanele[]>(_SkysenseDevContext.Paneles.Where(p => p.IdInstalacion == idInstalacion));
        }

        public Paginacion<Skysense_models.Respuestas.Documento> mObtenDocumentos(int idInstalacion, int paginaPaginacion, int registrosPorPagina, int tipoDocumento)
        {
            var dctos = this._mapper.Map<CDocumento[]>(_SkysenseDevContext.Documentos.Where(d => d.IdInstalacion == idInstalacion && d.TipoDocumento == tipoDocumento).ToArray());

            return new Paginacion<Skysense_models.Respuestas.Documento>()
            {
                paginaActual = paginaPaginacion,
                masDatos = dctos.Length == registrosPorPagina + 1,
                numeroPaginas = (int)Math.Ceiling((dctos.Length / (decimal)registrosPorPagina)),
                totalRegistros = dctos.Length,
                registrosPorPagina = registrosPorPagina,
                registros = dctos.Skip((paginaPaginacion - 1) * registrosPorPagina).Take(registrosPorPagina + 1).Select(d => new Skysense_models.Respuestas.Documento()
                {
                    fechaModificacion = d.FechaModificacion,
                    idInstalacion = d.IdInstalacion,
                    idDocumento = d.IdDocumento,
                    nombreDocumento = d.NombreDocumento,
                    tipoDocumento = d.TipoDocumento,
                    peso = d.Peso,
                    tipoArchivo = d.TipoArchivo
                }).Take(registrosPorPagina).ToArray()
            };
        }

        public async Task<int> mInsertaModificaDocumento(CDocumento dcto)
        {
            int idDocumento = 0;

            if (dcto == null)
            {
                throw new ErrorAlCliente("Nada que insertar.", 400);
            }

            //Si es modificacion
            if (dcto.IdDocumento > 0)
            {
                var dbDcto = this._SkysenseDevContext.Documentos.FirstOrDefault(d => d.IdInstalacion == dcto.IdInstalacion && d.IdDocumento == dcto.IdDocumento);
                if (dbDcto == null)
                    throw new ErrorAlCliente("Se está intentando actualizar un documento que no existe.", 400);

                dbDcto.Peso = dcto.Peso;
                dbDcto.TipoArchivo = dcto.TipoArchivo;
                dbDcto.FechaModificacion = DateTime.Now;
                dbDcto.NombreDocumento = dcto.NombreDocumento;
                dbDcto.RutaRelativa = dcto.RutaRelativa;

                await this._SkysenseDevContext.SaveChangesAsync();

                idDocumento = dbDcto.IdDocumento;
            }
            else
            {
                var doc = new Skysense_persistencia.Entidades.Documento()
                {
                    TipoArchivo = dcto.TipoArchivo,
                    FechaModificacion = DateTime.Now,
                    IdInstalacion = dcto.IdInstalacion,
                    NombreDocumento = dcto.NombreDocumento,
                    Peso = dcto.Peso,
                    RutaRelativa = dcto.RutaRelativa,
                    TipoDocumento = dcto.TipoDocumento
                };

                this._SkysenseDevContext.Documentos.Add(doc);

                await this._SkysenseDevContext.SaveChangesAsync();

                idDocumento = doc.IdDocumento;
            }

            return idDocumento;
        }

        public CDocumento fObtenDocumento(int idInstalacion, int idDocumento)
        {
            return this._mapper.Map<CDocumento>(this._SkysenseDevContext.Documentos.FirstOrDefault(d => d.IdInstalacion == idInstalacion && d.IdDocumento == idDocumento)
                ?? throw new ErrorAlCliente("No se encontró el documento solicitado.", 404));
        }

        public async Task mEliminaDocumento(int idInstalacion, int idDocumento)
        {
            var dcto = this._SkysenseDevContext.Documentos.FirstOrDefault(d => d.IdInstalacion == idInstalacion && d.IdDocumento == idDocumento);
            if (dcto == null)
                throw new ErrorAlCliente("No se encontró el documento solicitado.", 404);

            //Si el documento es de tipo recibo o reporte, se debe eliminar la referencia
            if (dcto.TipoDocumento == 1) //Reporte
            {
                var reporte = this._SkysenseDevContext.ReportesMensuales.FirstOrDefault(r => r.IdDocumento == dcto.IdDocumento);
                if (reporte != null)
                {
                    this._SkysenseDevContext.ReportesMensuales.Remove(reporte);
                }
            }
            else if (dcto.TipoDocumento == 2) //Recibo
            {
                var recibo = this._SkysenseDevContext.RecibosMensuales.FirstOrDefault(r => r.IdDocumento == dcto.IdDocumento);
                if (recibo != null)
                {
                    this._SkysenseDevContext.RecibosMensuales.Remove(recibo);
                }
            }

            this._SkysenseDevContext.Documentos.Remove(dcto);
            await this._SkysenseDevContext.SaveChangesAsync();

            return;
        }

        public CGrupo[] mObtenGrupos()
        {
            return _SkysenseDevContext.Grupos.Select(g => new CGrupo
            {
                IdGrupo = g.IdGrupo,
                Nombre = g.Nombre,
                Disponible = g.Disponible
            }).ToArray();
        }

        public void mModificaPanel(CPanele panele)
        {
            var panel = this._SkysenseDevContext.Paneles.FirstOrDefault(p => p.IdPanel == panele.IdPanel);

            if (panel == null)
            {
                throw new ErrorAlCliente("No se encontró el panel a modificar.", 404);
            }

            if (_SkysenseDevContext.Paneles.Any(i => i.IdPanel != panele.IdPanel && i.NumeroSerie == panele.NumeroSerie && i.Modelo == panele.Modelo))
            {
                throw new ErrorAlCliente("Se está intentando introducir un número de serie y modelo que otro Panel.");
            }

            panel.Danado = panele.Danado;
            panel.DegradacionAnual = panele.DegradacionAnual;
            //panel.IdEstadoPanel = panele.IdEstadoPanel;
            panel.Marca = panele.Marca;
            panel.Modelo = panele.Modelo;
            panel.NumeroSerie = panele.NumeroSerie;
            panel.NumeroSerieReemplazo = panele.NumeroSerieReemplazo;
            panel.Potencia = panele.Potencia;
            panel.ProveedorIntermediario = panele.ProveedorIntermediario;
            panel.ProveedorIntermediarioRfc = panele.ProveedorIntermediarioRfc;
            panel.ProveedorSuministrador = panele.ProveedorSuministrador;
            panel.ProveedorSuministradorRfc = panele.ProveedorSuministradorRfc;
            panel.EsInicial = panele.EsInicial;
            this._SkysenseDevContext.SaveChanges();
            return;

        }

        public void EliminarPanel(int idPanel)
        {
            var panel = this._SkysenseDevContext.Paneles.FirstOrDefault(p => p.IdPanel == idPanel);
            if (panel == null)
            {
                throw new ErrorAlCliente("No se encontró el panel a eliminar.", 404);
            }
            this._SkysenseDevContext.Paneles.Remove(panel);
            this._SkysenseDevContext.SaveChanges();
            return;
        }

        public CReporte[] mObtenReportesPorInstalacion(int idInstalacion, int iAnnio)
        {
            return this._mapper.Map<CReporte[]>(this._SkysenseDevContext.ReportesMensuales.Where(r => r.IdInstalacion == idInstalacion && r.MesReporte.Year == iAnnio).Include(r => r.IdDocumentoNavigation).ToArray());
        }
        public CRecibo[] mObtenRecibosPorInstalacion(int idInstalacion, int iAnnio)
        {
            return this._mapper.Map<CRecibo[]>(this._SkysenseDevContext.RecibosMensuales.Where(r => r.IdInstalacion == idInstalacion && r.MesRecibo.Year == iAnnio).Include(r => r.IdDocumentoNavigation).ToArray());
        }

        public async Task mInsertaRecibo(CRecibo cRecibo)
        {
            var reciboAnterior = this._SkysenseDevContext.RecibosMensuales.Where(r => r.IdInstalacion == cRecibo.IdInstalacion && r.MesRecibo == cRecibo.MesRecibo).FirstOrDefault();

            if (reciboAnterior == null)
            {
                this._SkysenseDevContext.RecibosMensuales.Add(new RecibosMensuale()
                {
                    IdInstalacion = cRecibo.IdInstalacion,
                    MesRecibo = cRecibo.MesRecibo,
                    IdDocumento = cRecibo.IdDocumento
                });

                await this._SkysenseDevContext.SaveChangesAsync();
            }
        }
        public async Task<int> mInsertaReporte(CReporte cReporte)
        {
            var reciboAnterior = this._SkysenseDevContext.ReportesMensuales.Where(r => r.IdInstalacion == cReporte.IdInstalacion && r.MesReporte == cReporte.MesReporte).FirstOrDefault();

            if (reciboAnterior == null)
            {
                var reporteNuevo = new ReportesMensuale()
                {
                    IdInstalacion = cReporte.IdInstalacion,
                    MesReporte = cReporte.MesReporte,
                    IdDocumento = cReporte.IdDocumento
                };

                this._SkysenseDevContext.ReportesMensuales.Add(reporteNuevo);

                await this._SkysenseDevContext.SaveChangesAsync();

                return reporteNuevo.IdReporte;
            }

            return -1;
        }

        public void mInsertaModificaReporte(CReporte reporte)
        {
            var dbReporte = this._SkysenseDevContext.ReportesMensuales.FirstOrDefault(r => r.IdInstalacion == reporte.IdInstalacion && r.IdReporte == reporte.IdReporte);
            if (dbReporte == null)
            {
                dbReporte = new ReportesMensuale()
                {
                    IdInstalacion = reporte.IdInstalacion,
                    MesReporte = reporte.MesReporte,
                    IdDocumento = reporte.IdDocumento
                };
            }

            dbReporte.IdDocumento = reporte.IdDocumento;
            dbReporte.AhorroAcumulado = reporte.AhorroAcumulado;
            dbReporte.AhorroAmbiental = reporte.AhorroAmbiental;
            dbReporte.ConsumoCfe = reporte.ConsumoCFE;
            dbReporte.PanelesGeneracion = reporte.PanelesGeneracion;

            this._SkysenseDevContext.SaveChanges();
        }

        public void mInsertaModificaRecibo(CRecibo recibo)
        {
            var dbReporte = this._SkysenseDevContext.RecibosMensuales.FirstOrDefault(r => r.IdInstalacion == recibo.IdInstalacion && r.IdRecibo == recibo.IdRecibo);
            if (dbReporte == null)
            {
                dbReporte = new RecibosMensuale()
                {
                    IdInstalacion = recibo.IdInstalacion,
                    MesRecibo = recibo.MesRecibo,
                    IdDocumento = recibo.IdDocumento
                };
            }

            dbReporte.IdDocumento = recibo.IdDocumento;
            dbReporte.KWbase = recibo.KWbase;
            dbReporte.KWintermedia = recibo.KWintermedia;
            dbReporte.KWpunta = recibo.KWpunta;
            dbReporte.KWhBase = recibo.KWhBase;
            dbReporte.KWhIntermedia = recibo.KWhIntermedia;
            dbReporte.KWhPunta = recibo.KWhPunta;
            dbReporte.ReactivosKvArh = recibo.ReactivosKvArh;
            dbReporte.Pago = recibo.Pago;

            this._SkysenseDevContext.SaveChanges();
        }

        public decimal?[] mObtenConsumoHistorico(int idInstalacion, int anno)
        {
            var consumo = this._SkysenseDevContext.RecibosMensuales
                .Where(r => r.IdInstalacion == idInstalacion && r.MesRecibo.Year == anno)
                .GroupBy(r => r.MesRecibo.Month)
                .Select(g => new
                {
                    Mes = g.Key,
                    Consumo = g.Sum(r => (r.KWhBase ?? 0) + (r.KWhIntermedia ?? 0) + (r.KWhPunta ?? 0))
                })
                .ToList();
            decimal?[] consumoMensual = new decimal?[12];
            foreach (var c in consumo)
            {
                consumoMensual[c.Mes - 1] = c.Consumo;
            }
            return consumoMensual;
        }

        public decimal?[][] mObtenFacturasHistorico(int idInstalacion, int anno)
        {
            decimal?[][] resp = new decimal?[3][];

            var consumo = this._SkysenseDevContext.RecibosMensuales
                .Where(r => r.IdInstalacion == idInstalacion && r.MesRecibo.Year <= anno && r.MesRecibo.Year > anno-3)
                .GroupBy(r => new { r.MesRecibo.Year })
                .Select(g => new
                {
                    Anno = g.Key.Year,
                    Meses = g.Select(s=>s.MesRecibo.Month).ToList(),
                    Pagos = g.Select(s => s.Pago)
                })
                .ToList();

            for(int i = 0; i < 3; i++)
            {
                resp[i] = new decimal?[12];
                var meses = consumo.FirstOrDefault(y => y.Anno == anno - i)?.Meses ?? [];
                var pagos = consumo.FirstOrDefault(y => y.Anno == anno - i)?.Pagos ?? [];
                for (int j = 0; j < meses.Count(); j++)
                {
                    resp[i][meses[j]-1] = pagos.ElementAt(j);
                }
            }

            return resp;
        }

        public decimal? ObtenAhorroAcumulado(int idInstalacion, int anno, int mes)
        {
            return _SkysenseDevContext.ReportesMensuales.FirstOrDefault(r => r.IdInstalacion == idInstalacion && r.MesReporte == new DateOnly(anno, mes - 1, 1))?.AhorroAcumulado;
        }

        public Task<CReporteAutomaticoConfig> mObtenConfiguracionReportesAutomaticos(int idInstalacion)
        {
            var config = this._SkysenseDevContext.ReporteAutomaticoConfigs.FirstOrDefault(r => r.IdInstalacion == idInstalacion);
            if (config == null)
            {
                config = new Skysense_persistencia.Entidades.ReporteAutomaticoConfig()
                {
                    IdInstalacion = idInstalacion,
                    NombreEnRecibo = this._SkysenseDevContext.Instalaciones.Find(idInstalacion)?.Nombre ?? "",
                    PorcentajeDap = 0.00m,
                    UmbralFp = 0.90m,
                };
                this._SkysenseDevContext.ReporteAutomaticoConfigs.Add(config);
                this._SkysenseDevContext.SaveChanges();
            }
            return Task.FromResult(this._mapper.Map<CReporteAutomaticoConfig>(config));
        }

        public async Task<bool> mGuardaConfiguracionReportesAutomaticos(int idInstalacion, CReporteAutomaticoConfig config)
        {
            var dbConfig = this._SkysenseDevContext.ReporteAutomaticoConfigs.FirstOrDefault(r => r.IdInstalacion == idInstalacion);
            if (dbConfig == null)
            {
                dbConfig = new Skysense_persistencia.Entidades.ReporteAutomaticoConfig()
                {
                    IdInstalacion = idInstalacion,
                };
                this._SkysenseDevContext.ReporteAutomaticoConfigs.Add(dbConfig);
            }
            dbConfig.NombreEnRecibo = config.NombreEnRecibo;
            dbConfig.PorcentajeDap = config.PorcentajeDap;
            dbConfig.UmbralFp = config.UmbralFp;
            dbConfig.FpDefault = config.FpDefault;
            return (await this._SkysenseDevContext.SaveChangesAsync()) > 0;
        }


        public CDocumento[] mObtenDocumentosTipo(int idInstalacion, int[] tipos)
        {
            return this._mapper.Map<CDocumento[]>(this._SkysenseDevContext.Documentos.Where(d => d.IdInstalacion == idInstalacion && tipos.Contains(d.TipoDocumento))
                .ToArray());
        }

        public Paginacion<PanelSimple> mObtenPanelesPaginados(int pagina, int registrosPorPagina, string? buscador)
        {
            var query = _SkysenseDevContext.Paneles.Include(i => i.IdInstalacionNavigation).AsQueryable();

            if (buscador != null)
            {
                query = query.Where(i => i.NumeroSerie.Contains(buscador) ||
                                         i.Marca.Contains(buscador) ||
                                         i.Modelo.Contains(buscador) ||
                                         (i.IdInstalacionNavigation.Nombre != null && i.IdInstalacionNavigation.Nombre.Contains(buscador)));

            }

            var totalRegistros = query.Count();

            var paneles = query.Skip((pagina - 1) * registrosPorPagina)
                                   .Take(registrosPorPagina)
                                   .ToList();

            return new Paginacion<PanelSimple>
            {
                paginaActual = pagina,
                registrosPorPagina = registrosPorPagina,
                numeroPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina),
                masDatos = (pagina * registrosPorPagina) < totalRegistros,
                totalRegistros = totalRegistros,
                registros = paneles.Select(i => new PanelSimple()
                {
                    idInstalacion = i.IdInstalacion,
                    idCliente = i.IdInstalacionNavigation.IdCliente,
                    instalacion = i.IdInstalacionNavigation.Nombre ?? "",
                    marca = i.Marca,
                    modelo = i.Modelo,
                    potencia = i.Potencia,
                    idPanel = i.IdPanel,
                    numeroSerie = i.NumeroSerie
                }).ToArray()
            };
        }

        public OpcionesCatalogo[] mModificaAgregaCatalogo(int idCatalogoMaestro, int idOpcionCatalogo, string opcion)
        {
            var catalogo = this._SkysenseDevContext.CatalogoOpciones.FirstOrDefault(c => c.IdCatalogo == idCatalogoMaestro && c.IdOpcion == idOpcionCatalogo);
            if (catalogo == null)
            {
                catalogo = new Skysense_persistencia.Entidades.CatalogoOpcione()
                {
                    IdCatalogo = idCatalogoMaestro,
                    IdOpcion = idOpcionCatalogo,
                    NombreOpcion = opcion
                };
                this._SkysenseDevContext.CatalogoOpciones.Add(catalogo);
            }
            else
            {
                catalogo.NombreOpcion = opcion;
            }
            this._SkysenseDevContext.SaveChanges();
            return this._consultasComunes.fcObtenerOpcionesCatalago(idCatalogoMaestro);
        }

        public bool mCambiaGrupo(int idInstalacion, int idGrupoN)
        {
            this._SkysenseDevContext.Instalaciones.First(i => i.IdInstalacion == idInstalacion).IdGrupo = idGrupoN != 0 ? (short?)idGrupoN: null;
            return this._SkysenseDevContext.SaveChanges() > 0;
        }

        public CInstalacione mObtenInstalacionCompleta(int idInstalacion)
        {
            return this._mapper.Map<CInstalacione>(this._SkysenseDevContext.Instalaciones
                .Where(i => i.IdInstalacion == idInstalacion)
                .Include(i => i.IdClienteNavigation)
                //.Include(i => i.Paneles)
                //.Include(i => i.Inversores)
                .Include(i => i.IdGrupoNavigation)
                .FirstOrDefault() ?? throw new ErrorAlCliente("No se encontró la instalación solicitada.", 404));
        }

        public void mActualizaValoresGarantizados(int idInstalacion, int iAnnio, CInstalacionApigeneracionMensual[] valoresGarantizados)
        {
            var meses = this._SkysenseDevContext.InstalacionApigeneracionMensuals.Where(gm => gm.IdInstalacion == idInstalacion && gm.Anno == iAnnio);

            foreach (var valG in valoresGarantizados)
            {
                var mes = meses.FirstOrDefault(m => m.Mes == valG.Mes);

                if (mes != null)
                {
                    mes.GeneracionGarantizada = valG.GeneracionGarantizada;
                }
                else
                {
                    mes = new InstalacionApigeneracionMensual()
                    {
                        IdInstalacion = idInstalacion,
                        DaemonYaEjecutado = false,
                        Anno = (short)iAnnio,
                        Mes = valG.Mes,
                        GeneracionGarantizada = valG.GeneracionGarantizada
                    };

                    this._SkysenseDevContext.InstalacionApigeneracionMensuals.Add(mes);
                }

                this._SkysenseDevContext.SaveChanges();
            }

        }

        public CInstalacionApigeneracionMensual[] mObtenValoresGarantizados(int idInstalacion, int iAnion)
        {
            return this._mapper.Map<CInstalacionApigeneracionMensual[]>(this._SkysenseDevContext.InstalacionApigeneracionMensuals
                .Where(gm => gm.IdInstalacion == idInstalacion && gm.Anno == iAnion).ToArray());
        }

        public CfObtenGeneracionMensualResult[] mObtenValoresRealesDB(int idInstalacion, int iAnion)
        {
            return this._mapper.Map<CfObtenGeneracionMensualResult[]>
                (this._SkysenseDevContext.fObtenGeneracionMensual(idInstalacion, (short)iAnion).ToArray());

        }

        public InversorDataDiaria[] mObtenGeneracionDiariaBD(int idInstalacion, int iAnio, int iMes)
        {

            return this._SkysenseDevContext.InversorApiencabezados
                .Where(i => i.IdInstalacion == idInstalacion)
                .Include(i => i.InversorApigeneracions).ToArray()
                .Select((i, index) => new InversorDataDiaria()
                {
                    encabezado = "Inversor " + index,
                    identificador = i.IdApi,
                    numeroSerie = i.NumeroSerie,
                    inversorData = i.InversorApigeneracions.Where(g => g.FechaValor.Year == iAnio && g.FechaValor.Month == iMes).Select(d =>
                        new InversorData()
                        {
                            fecha = d.FechaValor,
                            esEditado = d.ValorManual,
                            generacion = d.Valor
                        }).ToArray()
                }).ToArray();
        }

        public KPIInstalacionesResult[] mObtenKpiAnual(int anno, bool annoGarantia, int? mes)
        {
            return this._mapper.Map<KPIInstalacionesResult[]>(this._SkysenseDevContext.fObtenKPIInstalacionesAnualMensual((short)anno, (byte?)mes, !annoGarantia).ToArray());
        }


    }
}
