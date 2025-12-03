using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skysense_business.Auth.Interfaces;
using Skysense_models.DB;
using Skysense_models.Peticiones;
using Skysense_models.Respuestas;
using Skysense_persistencia.Entidades;
using System.Security.Claims;

namespace Skysense_app.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class InterfazController : Controller
    {

        private IInterfazBusiness _interf;

        public InterfazController(IInterfazBusiness interf)
        {
            _interf = interf;
        }

        /// <summary>
        /// Este método obtiene todos los catálogos maestros disponibles en el sistema. Sin información de sus opciones.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = RolesEnum.Administrador)]
        [HttpGet,Route("ObtenCatalogoDeCatalogos")]
        public ActionResult<ApiRespuesta<CCatalogoMaestro[]>> ObtenCatalogoDeCatalogos()
        {
            try
            {
                return new ApiRespuesta<CCatalogoMaestro[]>
                {
                    exito = true,
                    mensaje = "Catálogos maestros obtenidos correctamente.",
                    codigoError = 0,
                    data = _interf.mObtenCatalogosMaestros()
                };
            }catch(Exception ex)
            {
                return new ApiRespuesta<CCatalogoMaestro[]>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = null
                };
            }
        }

        
        [HttpGet, Route("ObtenCatalogosConOpciones/{sIdCatalogos}")]
        public ActionResult<ApiRespuesta<CCatalogoMaestro[]>> ObtenCatalogo([FromRoute]string sIdCatalogos)
        {
            try
            {
                var catalogo = _interf.mObtenCatalogoMaestro(sIdCatalogos.Split(",").Select(i => int.Parse(i)).ToArray());
                if (catalogo == null)
                {
                    return NotFound();
                }
                return new ApiRespuesta<CCatalogoMaestro[]>
                {
                    exito = true,
                    mensaje = "Catálogo maestro obtenido correctamente.",
                    codigoError = 0,
                    data = catalogo
                };
            }
            catch (Exception ex)
            {
                return new ApiRespuesta<CCatalogoMaestro[]>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = null
                };
            }
        }

        // POST: InterfazController/Create
        [Authorize(Roles = RolesEnum.Administrador)]
        [HttpPost, Route("ModificaCatalogo")]
        public ActionResult<ApiRespuesta<bool>> ModificaCatalogo([FromBody]CCatalogoOpcione opcionAModificar)
        {
            try
            {
                bool resultado = _interf.mActualizaCatalogoMaestro(opcionAModificar);
                return new ApiRespuesta<bool>
                {
                    exito = resultado,
                    mensaje = resultado ? "Catálogo modificado correctamente." : "Error al modificar el catálogo.",
                    codigoError = resultado ? 0 : -1,
                    data = resultado
                };
            }
            catch (Exception ex)
            {
                return new ApiRespuesta<bool>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = false
                };
            }
        }

        // POST: InterfazController/Create
        [Authorize(Roles = RolesEnum.Administrador)]
        [HttpPost, Route("EliminaOpcionCatalogo")]
        public ActionResult<ApiRespuesta<bool>> EliminaOpcionCatalogo([FromBody]CCatalogoOpcione opcionAEliminar)
        {
            try
            {
                bool resultado = _interf.mEliminaOpcionCatalogo(opcionAEliminar);
                return new ApiRespuesta<bool>
                {
                    exito = resultado,
                    mensaje = resultado ? "Registro eliminado correctamente." : "Error al eliminar el registro.",
                    codigoError = resultado ? 0 : -1,
                    data = resultado
                };
            }
            catch (Exception ex)
            {
                return new ApiRespuesta<bool>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = false
                };
            }
        }

        [HttpGet, Route("ObtenGrupos")]
        public ActionResult<ApiRespuesta<CGrupo[]>> ObtenGrupos()
        {
            try
            {
                return new ApiRespuesta<CGrupo[]>
                {
                    exito = true,
                    mensaje = "Grupos obtenidos correctamente.",
                    codigoError = 0,
                    data = _interf.mObtenGrupos()
                };
            }
            catch (Exception ex)
            {
                return new ApiRespuesta<CGrupo[]>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = null
                };
            }
        }

        // POST: InterfazController/Create
        [Authorize(Roles = RolesEnum.Administrador)]
        [HttpPost, Route("ModificaGrupo")]
        public ActionResult<ApiRespuesta<bool>> ModificaGrupo([FromBody] CGrupo opcionAModificar)
        {
            try
            {
                
                bool resultado = _interf.mActualizaGrupo(opcionAModificar);
                return new ApiRespuesta<bool>
                {
                    exito = resultado,
                    mensaje = resultado ? "Grupo modificado correctamente." : "Error al modificar el grupo.",
                    codigoError = resultado ? 0 : -1,
                    data = resultado
                };
            }
            catch (Exception ex)
            {
                return new ApiRespuesta<bool>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = false
                };
            }
        }

        // POST: InterfazController/Create
        [Authorize(Roles = RolesEnum.Administrador)]
        [HttpPost, Route("EliminaGrupo")]
        public ActionResult<ApiRespuesta<bool>> EliminaGrupo([FromBody] int idGrupo)
        {
            try
            {
                bool resultado = _interf.mEliminaGrupo(idGrupo);
                return new ApiRespuesta<bool>
                {
                    exito = resultado,
                    mensaje = resultado ? "Registro eliminado correctamente." : "Error al eliminar el registro.",
                    codigoError = resultado ? 0 : -1,
                    data = resultado
                };
            }
            catch (Exception ex)
            {
                return new ApiRespuesta<bool>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = false
                };
            }
        }

        [Authorize(Roles = RolesEnum.Administrador)]
        [HttpGet, Route("ObtenRolesSistema")]
        public ActionResult<ApiRespuesta<string[]>> ObtenRolesSistema()
        {
            try
            {
                return new ApiRespuesta<string[]>
                {
                    exito = true,
                    mensaje = "Roles del sistema obtenidos correctamente.",
                    codigoError = 0,
                    data = _interf.mObtenRolesSistema()
                };
            }
            catch (Exception ex)
            {
                return new ApiRespuesta<string[]>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = null
                };
            }
        }

        [Authorize(Roles = RolesEnum.Administrador)]
        [HttpGet, Route("ObtenUsuarios")]
        public ActionResult<ApiRespuesta<UserTabla[]>> ObtenUsuarios()
        {
            try
            {
                return new ApiRespuesta<UserTabla[]>
                {
                    exito = true,
                    mensaje = "Usuarios del sistema obtenidos correctamente.",
                    codigoError = 0,
                    data = _interf.mObtenUsuarios()
                };
            }
            catch (Exception ex)
            {
                return new ApiRespuesta<UserTabla[]>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = null
                };
            }
        }

        [Authorize(Roles = RolesEnum.Administrador)]
        [HttpPost, Route("ModificaUsuario")]
        public async Task<ActionResult<ApiRespuesta<bool>>> ModificaUsuario([FromBody] UserTabla usuarioAModificar)
        {
            try
            {
                bool resultado = await _interf.mActualizaUsuario(usuarioAModificar);
                return new ApiRespuesta<bool>
                {
                    exito = resultado,
                    mensaje = resultado ? "Usuario modificado correctamente." : "Error al modificar el usuario.",
                    codigoError = resultado ? 0 : -1,
                    data = resultado
                };
            }
            catch (Exception ex)
            {
                return new ApiRespuesta<bool>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = false
                };
            }
        }
        
        [Authorize]
        [HttpGet, Route("ObtenZonasTarifas/{iAnno}/{iMes}/{idZona}/{idTarifa?}")]
        public ActionResult<ApiRespuesta<CTarifasDivisione[]>> ObtenTarifas(int iAnno, int iMes, int idZona, int? idTarifa = null)
        {
            try
            {
                return new ApiRespuesta<CTarifasDivisione[]>
                {
                    exito = true,
                    mensaje = "Tarifas del sistema obtenidas correctamente.",
                    codigoError = 0,
                    data = _interf.mObtenTarifasDivisiones(iMes, iAnno, idZona, idTarifa)
                };
            }
            catch (Exception ex)
            {
                return new ApiRespuesta<CTarifasDivisione[]>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = null
                };
            }
        }

        [Authorize]
        [HttpPost, Route("GuardaZonaTarifa/{iAnno}/{iMes}/{idZona}")]
        public ActionResult<ApiRespuesta<bool>> GuardaZonaTarifa(int iAnno, int iMes, int idZona, CTarifasDivisione[] tarifas)
        {
            try
            {
                bool resultado = _interf.mGuardaTarifasDivisiones(iMes, iAnno, idZona, tarifas);
                return new ApiRespuesta<bool>
                {
                    exito = true,
                    mensaje = "Tarifas del sistema guardadas correctamente.",
                    codigoError = 0,
                    data = resultado
                };
            }
            catch (Exception ex)
            {
                return new ApiRespuesta<bool>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = false
                };
            }
        }

        [Authorize]
        [HttpGet, Route("ObtenDiasFestivos/{iAnno}")]
        public ActionResult<ApiRespuesta<DateOnly[]>> ObtenDiasFestivos(int iAnno)
        {
            try
            {
                var dias = _interf.mObtenDiasFestivos(iAnno);
                return new ApiRespuesta<DateOnly[]>
                {
                    exito = true,
                    mensaje = "Días festivos obtenidos correctamente.",
                    codigoError = 0,
                    data = dias
                };
            }
            catch (Exception ex)
            {
                return new ApiRespuesta<DateOnly[]>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = null
                };
            }
        }

        [Authorize(Roles = RolesEnum.Administrador)]
        [HttpPost, Route("GuardaDiasFestivos/{iAnno}")]
        public ActionResult<ApiRespuesta<bool>> GuardaDiasFestivos([FromBody] DateOnly[] dias, [FromRoute] int iAnno)
        {
            try
            {
                bool resultado = _interf.mGuardaDiasFestivos(iAnno,dias);
                return new ApiRespuesta<bool>
                {
                    exito = resultado,
                    mensaje = resultado ? "Días festivos guardados correctamente." : "Error al guardar los días festivos.",
                    codigoError = resultado ? 0 : -1,
                    data = resultado
                };
            }
            catch (Exception ex)
            {
                return new ApiRespuesta<bool>
                {
                    exito = false,
                    mensaje = ex.Message,
                    codigoError = -1,
                    data = false
                };
            }
        }
    }
}
