using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using Skysense_business.Auth.Interfaces;
using Skysense_models;
using Skysense_models.DB;
using Skysense_models.DB.FunctionReturns;
using Skysense_models.Errors;
using Skysense_models.Otros;
using Skysense_models.Peticiones;
using Skysense_models.Respuestas;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Text.Json.Nodes;

namespace Skysense_app.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InstalacionesController : ControllerBase
    {
        private readonly IInstalaciones _instalacionesLogic;
        private readonly IConfiguration _conf;

        public InstalacionesController(IInstalaciones instalacionesLogic, IConfiguration configuration)
        {
            this._instalacionesLogic = instalacionesLogic;
            this._conf = configuration;
        }

        [HttpGet, Route("Instalacion/{idInstalacion}")]
        [ProducesResponseType(typeof(ApiRespuesta<CInstalacione>), 200)]
        public ActionResult<ApiRespuesta<CInstalacione>> ObtenInstalacionCompleta(int idInstalacion)
        {
            try
            {
                var inst = _instalacionesLogic.mObtenInstalacionCompleta(idInstalacion);
                return Ok(new ApiRespuesta<CInstalacione>()
                {
                    codigoError = 0,
                    data = inst,
                    exito = true,
                    mensaje = "Instalación obtenida con éxito."
                });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // GET: Instalaciones/ObtenInstalaciones
        [HttpGet, Route("ObtenInstalaciones/{paginaPaginacion}/{registrosPorPagina}")]
        public ActionResult<ApiRespuesta<Paginacion<CInstalacione>>> ObtenInstalaciones([FromQuery] int? idGrupo, [FromQuery] string? busqueda,
            [FromRoute] int paginaPaginacion, [FromRoute] int registrosPorPagina)
        {
            try
            {
                var insta = _instalacionesLogic.mObtenInstalaciones(idGrupo, busqueda, paginaPaginacion, registrosPorPagina);
                return Ok(new ApiRespuesta<Paginacion<CInstalacione>>()
                {
                    codigoError = 0,
                    data = insta,
                    exito = true,
                    mensaje = "Instalaciones obtenidas con éxito."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: Instalaciones/ModificaInstalacion
        [HttpPost, Route("ModificaInstalacion")]
        [ProducesResponseType(typeof(ApiRespuesta<CInstalacione>), 200)]
        public ActionResult<ApiRespuesta<CInstalacione>> ModificaInstalacion([FromBody] CInstalacione instalacionActualizado)
        {
            try
            {
                var insta = _instalacionesLogic.mActualizaInstalacion(instalacionActualizado);
                return Ok(new ApiRespuesta<CInstalacione>()
                {
                    data = insta,
                    codigoError = 0,
                    exito = true,
                    mensaje = "Exito"
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<CInstalacione>()
                {
                    data = null,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: Instalaciones/NuevaInstalacion
        [HttpPost, Route("NuevaInstalacion")]
        [ProducesResponseType(typeof(ApiRespuesta<CInstalacione>), 200)]
        public ActionResult<ApiRespuesta<CInstalacione>> NuevaInstalacion([FromBody] CInstalacione instalacionNueva)
        {
            try
            {
                var insta = _instalacionesLogic.mNuevaInstalacion(instalacionNueva);
                return Ok(new ApiRespuesta<CInstalacione>()
                {
                    data = insta,
                    codigoError = 0,
                    exito = true,
                    mensaje = "Exito"
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<CInstalacione>()
                {
                    data = null,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Instalaciones/ModificaGrupo
        [HttpPost, Route("ModificaGrupo/{idInstalacion}/{idGrupoN}")]
        [ProducesResponseType(typeof(CInstalacione), 200)]
        public ActionResult<bool> ModificaGrupo(int idInstalacion, int idGrupoN)
        {
            try
            {
                var insta = _instalacionesLogic.mCambiaGrupo(idInstalacion, idGrupoN);
                return Ok(insta);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: Instalaciones/Estadisticas/Generacion/{idCliente}/{idInstalacion}/{iAnio}
        [HttpGet, Route("Estadisticas/Generacion/{idCliente}/{idInstalacion}/{iAnio}")]
        public async Task<IActionResult> ObtenGeneracion(int idCliente, int idInstalacion, int iAnio)
        {
            try
            {
                var insta = await _instalacionesLogic.mObtenGeneracion(idCliente, idInstalacion, iAnio);
                return Ok(insta);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: Instalaciones/Estadisticas/Generacion/{idCliente}/{idInstalacion}/{iAnio}/{iMes}
        [HttpGet, Route("Estadisticas/GeneracionDiaria/{idCliente}/{idInstalacion}/{iAnio}/{iMes}")]
        public async Task<IActionResult> ObtenGeneracionDiaria(int idCliente, int idInstalacion, int iAnio, int iMes)
        {
            try
            {
                var insta = await _instalacionesLogic.mObtenGeneracionDiaria(idCliente, idInstalacion, iAnio, iMes);
                return Ok(insta);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // POST: Instalaciones/Paneles/Insertar/{idInstalacion}
        [HttpPost, Route("Paneles/Insertar/{idInstalacion}")]
        public async Task<ActionResult<ApiRespuesta<bool>>> InsertarPaneles(int idInstalacion, [FromBody] CPanele[] paneles)
        {
            try
            {
                var insta = await _instalacionesLogic.mInsertaNuevosPaneles(idInstalacion, paneles);
                return new ApiRespuesta<bool>()
                {
                    codigoError = 0,
                    exito = true,
                    mensaje = "Paneles insertados correctamente",
                    data = insta.tData > 0
                };
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<bool>()
                {
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message,
                    data = false
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: Instalaciones/Paneles/Modificar
        [HttpPost, Route("Paneles/Modificar")]
        public IActionResult ModificarPaneles([FromBody] CPanele panele)
        {
            try
            {
                _instalacionesLogic.mModificaPanel(panele);
                return Ok();
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<bool>()
                {
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message,
                    data = false
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: Instalaciones/Paneles/{idInstalacion}
        [HttpGet, Route("Paneles/{idInstalacion}")]
        public ActionResult<ApiRespuesta<CPanele[]>> ObtenerPaneles(int idInstalacion)
        {
            try
            {
                var paneles = _instalacionesLogic.mObtenPaneles(idInstalacion);
                return Ok(new ApiRespuesta<CPanele[]>()
                {
                    data = paneles,
                    codigoError = 0,
                    exito = true,
                    mensaje = ""
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: Instalaciones/Documentos/{idInstalacion}
        [HttpGet, Route("Documentos/{idInstalacion}/{paginaPaginacion}/{registrosPorPagina}/{tipoDocumento}")]
        public ActionResult<ApiRespuesta<Paginacion<Documento>>> ObtenerDocumentos(int idInstalacion, int paginaPaginacion, int registrosPorPagina, int tipoDocumento)
        {
            try
            {
                var paneles = _instalacionesLogic.mObtenDocumentos(idInstalacion, paginaPaginacion, registrosPorPagina, tipoDocumento);
                return Ok(new ApiRespuesta<Paginacion<Documento>>()
                {
                    data = paneles,
                    codigoError = 0,
                    exito = true,
                    mensaje = ""
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<Paginacion<Documento>>()
                {
                    data = null,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: Instalaciones/Documento
        [HttpPost, Route("Documento")]
        public async Task<ActionResult<ApiRespuesta<Documento>>> InsertarModificarDocumento(
            [FromForm] string documentoJSON, [FromForm] string? iAnno,
             IFormFile archivo)
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                    throw new ErrorAlCliente("No se ha enviado ningún archivo.");


                var documento = JsonConvert.DeserializeObject<Documento>(documentoJSON);

                var fechaAnno = iAnno == null || iAnno == "null" ? null : (DateTime?)DateTime.Parse(iAnno);

                await _instalacionesLogic.mInsertaModificaDocumento(documento, archivo, iAnno == null || fechaAnno == null ? null : new DateOnly(fechaAnno.Value.Year, fechaAnno.Value.Month, 1));


                return Ok(new ApiRespuesta<Documento>
                {
                    exito = true,
                    mensaje = "Archivo subido correctamente.",
                    codigoError = 0,
                    data = new Documento
                    {
                        idInstalacion = documento.idInstalacion,
                        tipoDocumento = documento.tipoDocumento,
                        nombreDocumento = documento.nombreDocumento,
                        tipoArchivo = archivo.ContentType,
                        peso = (int)archivo.Length,
                        fechaModificacion = DateTime.Now
                    }
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<Paginacion<Documento>>()
                {
                    data = null,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: Instalaciones/DocumentoRemove
        [HttpPost, Route("DocumentoRemove/{IdInstalacion}/{IdDocumento}")]
        public async Task<ActionResult<ApiRespuesta<Documento>>> mRemoverDocumento(int IdInstalacion, int IdDocumento)
        {
            try
            {
                await _instalacionesLogic.mEliminaDocumento(IdInstalacion, IdDocumento);
                return Ok(new ApiRespuesta<Documento>
                {
                    exito = true,
                    mensaje = "Documento eliminado correctamente.",
                    codigoError = 0,
                    data = null
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<Paginacion<Documento>>()
                {
                    data = null,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route("DocumentoDownload/{idInstalacion}/{idDocumento}")]
        public async Task<IActionResult> DescargarDocumento(int idInstalacion, int idDocumento)
        {
            try
            {
                // Obtén el documento desde la base de datos
                CDocumento dcto = _instalacionesLogic.mObtenDocumento(idInstalacion, idDocumento);
                if (dcto == null || string.IsNullOrEmpty(dcto.RutaRelativa))
                    return NotFound("Documento no encontrado.");

                // Construye la ruta física
                var rutaBase = _conf["Rutas_Documentos:Instalaciones"];
                var rutaCompleta = Path.Combine(rutaBase, dcto.RutaRelativa);

                if (!System.IO.File.Exists(rutaCompleta))
                    throw new ErrorAlCliente("Archivo no existe en el servidor.", 404);

                var provider = new FileExtensionContentTypeProvider();
                string tipoMime;
                if (!provider.TryGetContentType(rutaCompleta, out tipoMime))
                {
                    tipoMime = "application/octet-stream"; // Valor por defecto si no se reconoce la extensión
                }

                var nombreDescarga = dcto.NombreDocumento + dcto.TipoArchivo;

                var bytes = await System.IO.File.ReadAllBytesAsync(rutaCompleta);
                return File(bytes, tipoMime, nombreDescarga);
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<Paginacion<Documento>>()
                {
                    data = null,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // GET: api/Instalaciones/Grupos
        [HttpGet, Route("Grupos")]
        [ProducesResponseType(typeof(CGrupo[]), 200)]
        public ActionResult<ApiRespuesta<CGrupo[]>> ObtenerGrupos()
        {
            try
            {
                var grupos = _instalacionesLogic.mObtenGrupos();
                return Ok(new ApiRespuesta<CGrupo[]>()
                {
                    codigoError = 0,
                    data = grupos,
                    exito = true,
                    mensaje = ""
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route("Paneles/Eliminar/{idPanel}")]
        public ActionResult<ApiRespuesta<bool>> EliminarPanel(int idPanel)
        {
            try
            {
                // Lógica para eliminar el panel
                _instalacionesLogic.EliminarPanel(idPanel);
                return Ok(new ApiRespuesta<bool>()
                {
                    codigoError = 0,
                    data = true,
                    exito = true,
                    mensaje = "Panel eliminado correctamente."
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<bool>()
                {
                    data = false,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route("Reportes/{idInstalacion}/{iAnno}")]
        public ActionResult<ApiRespuesta<CReporte[]>> ObtenerReportesPorInstalacion(int idInstalacion, int iAnno)
        {
            try
            {
                var reportes = _instalacionesLogic.mObtenReportesPorInstalacion(idInstalacion, iAnno);
                return Ok(new ApiRespuesta<CReporte[]>()
                {
                    codigoError = 0,
                    data = reportes,
                    exito = true,
                    mensaje = ""
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route("Recibos/{idInstalacion}/{iAnnio}")]
        public ActionResult<ApiRespuesta<CRecibo[]>> ObtenerRecibosPorInstalacion(int idInstalacion, int iAnnio)
        {
            try
            {
                var recibos = _instalacionesLogic.mObtenRecibosPorInstalacion(idInstalacion, iAnnio);
                return Ok(new ApiRespuesta<CRecibo[]>()
                {
                    codigoError = 0,
                    data = recibos,
                    exito = true,
                    mensaje = ""
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route("Reporte")]
        public ActionResult<ApiRespuesta<bool>> InsertaModificaInfoReporte([FromBody] CReporte reporte)
        {
            try
            {
                _instalacionesLogic.mInsertaModificaReporte(reporte);
                return Ok(new ApiRespuesta<bool>()
                {
                    data = true,
                    exito = true,
                    mensaje = "Reporte insertado/modificado correctamente.",
                    codigoError = 0
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<bool>()
                {
                    data = false,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost, Route("Recibo")]
        public ActionResult<ApiRespuesta<bool>> InsertaModificaInfoRecibo([FromBody] CRecibo recibo)
        {
            try
            {
                _instalacionesLogic.mInsertaModificaRecibo(recibo);
                return Ok(new ApiRespuesta<bool>()
                {
                    data = true,
                    exito = true,
                    mensaje = "Recibo insertado/modificado correctamente.",
                    codigoError = 0
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<bool>()
                {
                    data = false,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpGet, Route("DocumentosTipo/{idInstalacion}/{sTipos}")]
        public ActionResult<ApiRespuesta<CDocumento[]>> ObtenerRecibosDeMismoTipoEInstalacion(int idInstalacion, string sTipos)
        {
            try
            {
                var docs = _instalacionesLogic.mObtenDocumentosTipo(idInstalacion, sTipos.Split(",").Select(t => int.Parse(t)).ToArray());
                return Ok(new ApiRespuesta<CDocumento[]>()
                {
                    codigoError = 0,
                    data = docs,
                    exito = true,
                    mensaje = ""
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: Inversores/ObtenInversores
        [HttpGet, Route("ObtenPanelesPaginados/{pagina}/{registrosPorPagina}")]
        [ProducesResponseType(typeof(ApiRespuesta<Paginacion<PanelSimple>>), 200)]
        public ActionResult<ApiRespuesta<Paginacion<PanelSimple>>> ObtenPanelesPaginados([FromRoute] int pagina, [FromRoute] int registrosPorPagina, [FromQuery] string? buscador)
        {
            try
            {
                var inv = _instalacionesLogic.mObtenPanelesPaginados(pagina, registrosPorPagina, buscador);
                return new ApiRespuesta<Paginacion<PanelSimple>>()
                {
                    codigoError = 0,
                    exito = true,
                    mensaje = "Paneles obtenidos exitosamente.",
                    data = inv
                };
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost, Route("ModificaAgregaCatalogo/{idCatalogoMaestro}/{idOpcionCatalogo}")]
        public ActionResult<ApiRespuesta<OpcionesCatalogo[]>> ModificaCatalogo([FromRoute] int idCatalogoMaestro, [FromRoute] int idOpcionCatalogo, [FromBody] string opcionModificada)
        {
            try
            {
                var resultado = _instalacionesLogic.mModificaAgregaCatalogo(idCatalogoMaestro, idOpcionCatalogo, opcionModificada);
                return Ok(new ApiRespuesta<OpcionesCatalogo[]>()
                {
                    codigoError = 0,
                    data = resultado,
                    exito = true,
                    mensaje = "Catálogo modificado/agregado correctamente."
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<OpcionesCatalogo[]>()
                {
                    data = null,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost, Route("Garantizados/{idInstalacion}/{iAnnio}")]
        public ActionResult<ApiRespuesta<bool>> ModificaGarantizados([FromRoute] int idInstalacion, [FromRoute] int iAnnio, [FromBody] CInstalacionApigeneracionMensual[] valoresGarantizados)
        {
            try
            {
                _instalacionesLogic.mActualizaValoresGarantizados(idInstalacion, iAnnio, valoresGarantizados);
                return Ok(new ApiRespuesta<bool>()
                {
                    data = true,
                    exito = true,
                    mensaje = "Reporte insertado/modificado correctamente.",
                    codigoError = 0
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<bool>()
                {
                    data = false,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route("ObtenKPIInstalaciones/{Anno}/{AnnoGarantia}/{Mes?}")]
        public ActionResult<ApiRespuesta<KPIInstalacionesResult[]>> ObtenKPIInstalaciones([FromRoute] int Anno, [FromRoute] bool AnnoGarantia, [FromRoute] int? Mes)
        {
            try
            {
                var inv = _instalacionesLogic.mObtenKpiAnual(Anno, AnnoGarantia, Mes);
                return Ok(new ApiRespuesta<KPIInstalacionesResult[]>()
                {
                    codigoError = 0,
                    exito = true,
                    mensaje = "KPI Exitoso.",
                    data = inv
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<KPIInstalacionesResult>()
                {
                    data = null,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = "Error al obtener KPI de Instalaciones: " + ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet, Route("ObtenDatosReporteMensual/{IdInstalacion}/{Anno}/{Mes}")]
        public async Task<ActionResult<ApiRespuesta<ReporteDatos>>> ObtenDatosReporteMensual([FromRoute] int IdInstalacion, [FromRoute] int Anno, [FromRoute] int Mes)
        {
            try
            {
                var inv = await _instalacionesLogic.mObtenDatosReporteMensual(IdInstalacion, Anno, Mes);
                return Ok(new ApiRespuesta<ReporteDatos>()
                {
                    codigoError = 0,
                    exito = true,
                    mensaje = "Datos del reporte mensual exitoso.",
                    data = inv
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<ReporteDatos>()
                {
                    data = null,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = "Error al obtener datos del reporte mensual: " + ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpPost, Route("GuardaReporteAutomatico/{idInstalacion}")]
        public async Task<ActionResult<ApiRespuesta<bool>>> SubeReporteAutomatico([FromRoute] int idInstalacion, [FromForm] string archivo, [FromForm] string Fecha, 
        
        [FromForm] decimal panelGeneracion, [FromForm] decimal ahorroAcumulado, [FromForm] decimal ahorroAmbiental, [FromForm] decimal consumoCFE)
        {
            try
            {
                var archivoBytes = Convert.FromBase64String(archivo);
                var archivoStream = new MemoryStream(archivoBytes);
                var archivoFormFile = new FormFile(archivoStream, 0, archivoBytes.Length, "file", "Reporte_"+Fecha.Replace("/","-")+ ".pdf")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/pdf"
                };

                bool bajaTension2 = false;
                if (Request.Form.ContainsKey("bajaTension2"))
                {
                    bool.TryParse(Request.Form["bajaTension2"], out bajaTension2);
                }

                await _instalacionesLogic.mSubeReporteAutomatico(idInstalacion, archivoFormFile, DateOnly.Parse(Fecha), panelGeneracion, ahorroAcumulado, ahorroAmbiental, consumoCFE, bajaTension2);
                return Ok(new ApiRespuesta<bool>()
                {
                    data = true,
                    exito = true,
                    mensaje = "Reporte automático insertado correctamente.",
                    codigoError = 0
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<bool>()
                {
                    data = false,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route("ConfiguracionReportesAutomaticos/{IdInstalacion}")]
        public async Task<ActionResult<ApiRespuesta<CReporteAutomaticoConfig>>> ObtenConfiguracionReportesAutomaticos([FromRoute] int IdInstalacion)
        {
            try
            {
                var inv = await _instalacionesLogic.mObtenConfiguracionReportesAutomaticos(IdInstalacion);
                return Ok(new ApiRespuesta<CReporteAutomaticoConfig>()
                {
                    codigoError = 0,
                    exito = true,
                    mensaje = "Datos de la configuración de reportes automáticos obtenidos exitosamente.",
                    data = inv
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<ReporteDatos>()
                {
                    data = null,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = "Error al obtener la configuración de reportes automáticos: " + ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route("ConfiguracionReportesAutomaticos/{IdInstalacion}")]
        public async Task<ActionResult<ApiRespuesta<CReporteAutomaticoConfig>>> GuardaConfiguracionReporteAutomatico([FromRoute] int IdInstalacion, [FromBody] CReporteAutomaticoConfig config)
        {
            try
            {
                var inv = await _instalacionesLogic.mGuardaConfiguracionReportesAutomaticos(IdInstalacion, config);
                return Ok(new ApiRespuesta<bool>()
                {
                    codigoError = 0,
                    exito = true,
                    mensaje = "Datos de la configuración de reportes automáticos guardados exitosamente.",
                    data = inv
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<bool>()
                {
                    data = false,
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = "Error al guardar la configuración de reportes automáticos: " + ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
