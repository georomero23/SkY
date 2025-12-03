using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Skysense_business.Auth.Interfaces;
using Skysense_models;
using Skysense_models.DB;
using Skysense_models.Errors;
using Skysense_models.Otros;
using Skysense_models.Respuestas;
using System.Security.Claims;

namespace Skysense_app.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InversoresController : ControllerBase
    {
        private readonly IInversores _inversoresLogic;

        public InversoresController(IInversores inversoresLogic)
        {
            this._inversoresLogic = inversoresLogic;
        }



        // GET: Inversores/ObtenInversores
        [HttpGet, Route("ObtenInversores/{idInstalacion}")]
        [ProducesResponseType(typeof(CInversore[]), 200)]
        public ActionResult<CInversore[]> ObtenInversores([FromRoute] int idInstalacion)
        {
            try
            {
                var inv = _inversoresLogic.mObtenInversores(idInstalacion);
                return inv;
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        // GET: Inversores/ObtenInversores
        [HttpGet, Route("ObtenInversoresPaginados/{pagina}/{registrosPorPagina}")]
        [ProducesResponseType(typeof(ApiRespuesta<Paginacion<InversorSimple>>), 200)]
        public ActionResult<ApiRespuesta<Paginacion<InversorSimple>>> ObtenInversoresPaginados([FromRoute] int pagina, [FromRoute] int registrosPorPagina, [FromQuery] string? buscador)
        {
            try
            {
                var inv = _inversoresLogic.mObtenInversoresPaginados(pagina, registrosPorPagina, buscador);
                return new ApiRespuesta<Paginacion<InversorSimple>>()
                {
                    codigoError = 0,
                    exito = true,
                    mensaje = "Inversores obtenidos exitosamente.",
                    data = inv
                };
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // POST: Inversores/ModificaInversor
        [HttpPost, Route("ModificaInserta/{accion}/{idInstalacion}")]
        [ProducesResponseType(typeof(CInversore), 200)]
        public ActionResult<ApiRespuesta<bool>> ModificaInversores([FromBody] CInversore[] inversorActualizado, [FromRoute] int accion, [FromRoute] int idInstalacion)
        {
            try
            {
                if (inversorActualizado == null || inversorActualizado.Length == 0)
                {
                    return BadRequest("No se recibieron datos para actualizar.");
                }
                if (accion < 0 || accion > 1)
                {
                    return BadRequest("Acción no válida. Use 0 para actualizar o 1 para insertar.");
                }
                if (accion == 0 && inversorActualizado.Length > 1)
                {
                    return BadRequest("Solo se puede modificar un inversor a la vez.");
                }
                if (accion == 1 && inversorActualizado.Length < 1)
                {
                    return BadRequest("Debe proporcionar al menos un inversor para insertar.");
                }
                if (accion == 1)
                {
                    var invs = _inversoresLogic.mNuevosInversores(idInstalacion, inversorActualizado);
                    return Ok(new ApiRespuesta<bool>()
                    {
                        codigoError = 0,
                        exito = true,
                        mensaje = "Inversores insertados exitosamente.",
                        data = invs
                    });
                }
                if (accion == 0)
                {
                    var inv = _inversoresLogic.mActualizaInversor(inversorActualizado[0]);
                    if (inv == null)
                    {
                        return BadRequest("No se encontró el inversor para actualizar.");
                    }
                    return Ok(new ApiRespuesta<bool>()
                    {
                        codigoError = 0,
                        exito = true,
                        mensaje = "Inversor actualizado exitosamente.",
                        data = true
                    });

                }

                return BadRequest("Acción no válida. Use 0 para actualizar o 1 para insertar.");
            }
            catch(ErrorAlCliente ex)
            {
                return new ApiRespuesta<bool>()
                {
                    codigoError = -1,
                    exito = false,
                    mensaje = ex.Message,
                    data = false
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: Inversores/ModificaInversor
        [Authorize(Roles = "Administrador,Operador Nivel 01")]
        [HttpPost, Route("AjustarDatosInversor")]
        [ProducesResponseType(typeof(ApiRespuesta<bool>), 200)]
        public async Task<ActionResult<ApiRespuesta<bool>>> AjustarDatosInversor([FromBody] Skysense_models.DB.CDatosInversoresAjustado[] apiGen)
        {
            try
            {
                var inv = await _inversoresLogic.mAjustaDatosInversor(apiGen);

                return new ApiRespuesta<bool>()
                {
                    codigoError = 0,
                    exito = true,
                    mensaje = "Datos guardados exitosamente.",
                    data = true
                };
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = RolesEnum.Administrador)]
        [HttpPost, Route("EliminarInversor/{idInversor}")]
        [ProducesResponseType(typeof(ApiRespuesta<bool>), 200)]
        public ActionResult<ApiRespuesta<bool>> EliminarInversor([FromRoute] int idInversor)
        {
            try
            {
                var result = _inversoresLogic.mEliminarInversor(idInversor);
                if (!result)
                {
                    return BadRequest("No se pudo eliminar el inversor.");
                }
                return Ok(new ApiRespuesta<bool>()
                {
                    codigoError = 0,
                    exito = true,
                    mensaje = "Inversor eliminado exitosamente.",
                    data = true
                });
            }
            catch(ErrorAlCliente ex)
            {
                return Ok( new ApiRespuesta<bool>()
                {
                    codigoError = ex.ErrorCode,
                    exito = false,
                    mensaje = ex.Message,
                    data = false
                });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

    }
}
