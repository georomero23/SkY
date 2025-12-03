using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skysense_business.Auth.Interfaces;
using Skysense_models.DB;
using Skysense_models.Respuestas;
using Skysense_persistencia.Entidades;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Skysense_app.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _authLogic;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AuthController(IAuth authLogic, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._authLogic = authLogic;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }



        // POST: Auth/fAcceso
        [AllowAnonymous]
        [HttpPost, Route("fAcceso")]
        [ProducesResponseType(typeof(CUser), 200)]
        public async Task<ActionResult<CUser?>> fAcceso([FromBody] CUser usuario)
        {
            try
            {
                var cPuedoAcceder = _authLogic.fAcceso(usuario);

                if (cPuedoAcceder != null)
                {
                    if (!cPuedoAcceder.UsCambiaCntrsn)
                    {
                        await HttpContext.SignInAsync("cookie", new System.Security.Claims.ClaimsPrincipal(
                                new ClaimsIdentity(
                                    new List<Claim>
                                    {
                                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                                    new Claim(ClaimTypes.Role, (usuario.IdRol??0).ToString())
                                    }, CookieAuthenticationDefaults.AuthenticationScheme
                                    )
                            ));
                    }
                }

                return cPuedoAcceder == null ? BadRequest("Error al iniciar sesión.") : Ok(cPuedoAcceder);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al iniciar sesión.");
            }
        }

        [AllowAnonymous]
        [HttpPost, Route("fChange")]
        [ProducesResponseType(typeof(CUser), 200)]
        public async Task<ActionResult<bool>> fCambiaContrasena([FromForm] int IdUsuario, [FromForm] string sAnterior, [FromForm] string sNueva)
        {
            try
            {
                var resp = await _authLogic.mCambiaContrasena(IdUsuario, sAnterior, sNueva);

                return Ok(false);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al iniciar sesión.");
            }
        }

        // POST: Auth/fSalir
        [HttpPost, Route("fSalir")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<ActionResult<bool>> fSalir()
        {
            try
            {
                await HttpContext.SignOutAsync("cookie");

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al cerrar sesión.");
            }
        }

        // GET: Auth/Me
        [HttpGet, Route("Me")]
        public async Task<ActionResult<ApiRespuesta<Usuario>>> ObtenInformacionDelUsuarioLogueado()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(userId);
                var role = await _userManager.GetRolesAsync(user);

                return new ApiRespuesta<Usuario>()
                {
                    codigoError = 0,
                    exito = true,
                    mensaje = "",
                    data = new Usuario()
                    {
                        id = user.Id,
                        mail = user.Email,
                        name = user.UserName,
                        roles = role.ToArray()
                    }
                };

            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener información del usuario.");
            }
        }

        // POST: Auth/Nuevo
        [HttpPost, Route("Nuevo")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<ActionResult<bool>> InsertaNuevoUsuario([FromBody] CUser usuario)
        {
            try
            {
                var respuesta = await _authLogic.mInsertaNuevoUsuario(usuario);

                if (respuesta.bExito)
                {
                    return Ok(true);
                }
                else
                {
                    return BadRequest("Ocurrió un error al insertar el nuevo usuario");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Ocurrió un error al insertar el nuevo usuario");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, Route("CambiaRoles")]
        public async Task<ActionResult<bool>> fCambiaRolesUsuario([FromBody] Usuario usuario)
        {
            try
            {
                //var userId = User.FindFirstValue(ClaimTypes.Name);
                var user = await _userManager.FindByEmailAsync(usuario.mail);
                await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                await _userManager.AddToRolesAsync(user, usuario.roles);
                return true;
            }
            catch (Exception ex)
            {
                return BadRequest("Ocurrió un error al cambiar los roles.");
            }
        }

        [HttpGet, Route("MisRoles")]
        public async Task<ActionResult<string[]>> fObtenRolesUsuario()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(userId);
                var role = await _userManager.GetRolesAsync(user);
                return role.ToArray();
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener los roles del usuario.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet, Route("Roles")]
        public async Task<ActionResult<string[]>> fObtenRolesSistema()
        {
            try
            {
                return await _roleManager.Roles.Select(r => r.Name ?? "").ToArrayAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener los roles del usuario.");
            }
        }
    }
}
