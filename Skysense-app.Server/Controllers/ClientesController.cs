using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skysense_business.Auth.Interfaces;
using Skysense_models.DB;
using Skysense_models.Errors;
using Skysense_models.Respuestas;

namespace Skysense_app.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ICLiente _clienteLogic;
        private readonly Skysense_business.Dashboard.IDashboard _dashboardLogic;

        public ClientesController(ICLiente clienteLogic, Skysense_business.Dashboard.IDashboard dashboardLogic)
        {
            this._clienteLogic = clienteLogic;
            this._dashboardLogic = dashboardLogic;
        }



        // GET: Clientes/ObtenClientes
        [HttpGet, Route("ObtenClientes")]
        [ProducesResponseType(typeof(ApiRespuesta<Paginacion<CCliente>>), 200)]
        public ActionResult<ApiRespuesta<Paginacion<CCliente>>> ObtenClientes([FromQuery] int numeroPagina, [FromQuery] int registrosPorPagina, [FromQuery] string? busqueda)
        {
            try
            {
                var clientes = _clienteLogic.mObtenClientes(numeroPagina, registrosPorPagina, busqueda);
                return Ok(new ApiRespuesta<Paginacion<CCliente>>()
                {
                    exito = true,
                    mensaje = "Clientes obtenidos correctamente",
                    codigoError = 0,
                    data = clientes
                });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: Clientes/ModificaCliente
        [HttpPost, Route("ModificaCliente")]
        [ProducesResponseType(typeof(ApiRespuesta<CCliente>), 200)]
        public ActionResult<ApiRespuesta<CCliente>> ModificaCliente([FromBody] CCliente clienteActualizado)
        {
            try
            {
                var cliente = _clienteLogic.mActualizaCliente(clienteActualizado);
                return Ok(new ApiRespuesta<CCliente>()
                {
                    exito = cliente != null,
                    mensaje = cliente != null ? "Cliente actualizado correctamente" : "No se encontró el cliente",
                    codigoError = cliente != null ? 0 : -1,
                    data = cliente
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<CCliente>()
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = ex.ErrorCode,
                    data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: Clientes/NuevoCliente
        [HttpPost, Route("NuevoCliente")]
        [ProducesResponseType(typeof(ApiRespuesta<CCliente>), 200)]
        public ActionResult<ApiRespuesta<CCliente>> NuevoCliente([FromBody] CCliente clienteNuevo)
        {
            try
            {
                var cliente = _clienteLogic.mNuevoCliente(clienteNuevo);
                return Ok(new ApiRespuesta<CCliente>()
                {
                    exito = cliente != null,
                    mensaje = cliente != null ? "Cliente actualizado correctamente" : "No se encontró el cliente",
                    codigoError = cliente != null ? 0 : -1,
                    data = cliente
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<CCliente>()
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = ex.ErrorCode,
                    data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: Clientes/FusionarClientes
        [HttpPost, Route("FusionarClientes")]
        [ProducesResponseType(typeof(ApiRespuesta<bool>), 200)]
        public async Task<ActionResult<ApiRespuesta<bool>>> FusionarClientes([FromBody] CCliente clienteFusion)
        {
            try
            {
                var cliente = await _clienteLogic.mFusionaClientes(clienteFusion.IdCliente, clienteFusion.Rfc);
                return Ok(new ApiRespuesta<bool>()
                {
                    exito = cliente != null,
                    mensaje = cliente != null ? cliente : "No se encontró el cliente",
                    codigoError = cliente != null ? 0 : -1,
                    data = true
                });
            }
            catch (ErrorAlCliente ex)
            {
                return Ok(new ApiRespuesta<bool>()
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = ex.ErrorCode,
                    data = false
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route("Cliente/{idCliente}")]
        [ProducesResponseType(typeof(Skysense_models.Otros.ClienteDashboard), 200)]
        public ActionResult<Skysense_models.Otros.ClienteDashboard?> ObtenCliente(int idCliente)
        {
            try
            {
                var cliente = _dashboardLogic.fObtieneClienteDashboard(idCliente);
                return cliente;
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet, Route("Instalacion/{idCliente}/{idInstalacion}")]
        [ProducesResponseType(typeof(Skysense_models.Otros.InstalacionDashboard), 200)]
        public ActionResult<Skysense_models.Otros.InstalacionDashboard?> ObtenInstalacion(int idCliente, int idInstalacion)
        {
            try
            {
                var inst = _dashboardLogic.fObtieneInstalacionDashboard(idCliente, idInstalacion);
                return inst;
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

    }
}
