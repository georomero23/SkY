using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skysense_app.Server.BackgroundServices;
using Skysense_business.Baterias;
using Skysense_models.BackgroundService;
using Skysense_models.Errors;
using Skysense_models.Otros;
using Skysense_models.Respuestas;

namespace Skysense_app.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BateriasController : Controller
    {
        IBateriasNegocio _bateriasNegocio;
        IOPCDataPolling _backgroundService;

        public BateriasController(IBateriasNegocio negocio, IOPCDataPolling oPCDataPolling) { 
            _bateriasNegocio = negocio;
            _backgroundService = oPCDataPolling;
        }

        [HttpGet("ObtenTagsOpciones")]
        public ActionResult<ApiRespuesta<TagSelectOptions[]>> ObtenOpcionesTags()
        {
            try
            {
                var bat = _bateriasNegocio.ObtenTiposTags();
                return new ApiRespuesta<TagSelectOptions[]>
                {
                    exito = true,
                    data = bat,
                    mensaje="Opciones obtenidas con éxito."
                };
            }
            catch (ErrorAlCliente ex)
            {
                return new ApiRespuesta<TagSelectOptions[]>
                {
                    exito = false,
                    data = null,
                    mensaje = ex.Message,
                    codigoError = -1
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ObtenConfiguracionBateria/{idInstalacion}")]
        public ActionResult<ApiRespuesta<OPCBateria>> ObtenConfiguracionBateria(int idInstalacion)
        {
            try
            {
                var bat = _bateriasNegocio.ObtenConfiguracionBateria(idInstalacion);
                return new ApiRespuesta<OPCBateria>
                {
                    exito = true,
                    data = bat,
                    mensaje = "Configuración obtenida con éxito."
                };
            }
            catch (ErrorAlCliente ex)
            {
                return new ApiRespuesta<OPCBateria>
                {
                    exito = false,
                    data = null,
                    mensaje = ex.Message,
                    codigoError = -1
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GuardaConfiguracionBateria/{idInstalacion}")]
        public async Task< ActionResult<ApiRespuesta<bool>>> GuardaConfiguracionBateria(int idInstalacion, [FromBody] OPCBateria bateriaN)
        {
            try
            {
                var bat = await _bateriasNegocio.GuardaConfiguracionBateria(bateriaN);

                if (bat)
                {
                    _ = _backgroundService.ActualizaMonitoreo(idInstalacion);
                }

                return new ApiRespuesta<bool>
                {
                    exito = true,
                    data = bat,
                    mensaje = "Configuración guardada con éxito."
                };
            }
            catch (ErrorAlCliente ex)
            {
                return new ApiRespuesta<bool>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ObtenTablaBaterias")]
        public ActionResult<ApiRespuesta<BateriasTabla>> ObtenTablaBaterias([FromQuery]string busqueda)
        {
            try
            {
                var bat = _bateriasNegocio.ObtenTablaBaterias(busqueda);
                return new ApiRespuesta<BateriasTabla>
                {
                    exito = true,
                    data = bat,
                    mensaje = "Configuración obtenida con éxito."
                };
            }
            catch (ErrorAlCliente ex)
            {
                return new ApiRespuesta<BateriasTabla>
                {
                    exito = false,
                    data = null,
                    mensaje = ex.Message,
                    codigoError = -1
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
