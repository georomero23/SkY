using Skysense_business.Auth.Interfaces;
using Skysense_business.Comun;
using Skysense_models.DB;
using Skysense_models.Otros;
using System.DirectoryServices.Protocols;
using System.Net;

namespace Skysense_business.Auth.Implementation
{
    public class AuthClass : IAuth
    {
        private readonly Skysense_Data.Auth.Interfaces.IAuth _auth;

        public AuthClass(Skysense_Data.Auth.Interfaces.IAuth auth)
        {
            this._auth = auth;
        }

        public CUser? fAcceso(CUser Usuario)
        {
            if (_auth.fAcceso(Usuario))
            {
                var ret = _auth.fObtenDatosDelUsuario(Usuario.UsCorreo);
                ret.UsCntrsn = "";
            }

            return null;
        }

        public async Task<MethodResponse<int>> mInsertaNuevoUsuario(CUser usuario)
        {
            if(usuario == null || (usuario.UsNombre??"").Length == 0 || (usuario.UsCorreo ?? "").Length == 0)
            {
                return new MethodResponse<int>(false, "No se admiten valores vacíos para nombre ni correo.");
            }
            else
            {
                MethodResponse<int> respuestaUsuario = await _auth.mInsertaNuevoUsuario(usuario, MetodosComunes.GeneraCodigoAleatorio(8));

                return respuestaUsuario;
            }
        }

        public async Task<MethodResponse<bool>> mCambiaContrasena(int idUsuario, string sAnterior, string sNueva)
        {
            var user = _auth.fObtenDatosDelUsuario(idUsuario);
            if(user.UsCntrsn == sAnterior)
            {
                return await Task.FromResult(new MethodResponse<bool>(false, "La contraseña anterior no es correcta.", 1, false));
            }
            else
            {
                return await Task.FromResult(new MethodResponse<bool>(false, "La contraseña anterior no es correcta.", 1, false));
            }
        }

        CUser? IAuth.mObtenInfoUsuario(string? idUsuario)
        {
            if (int.TryParse(idUsuario,out int idUsuarioInt))
            {
                return _auth.fObtenDatosDelUsuario(idUsuarioInt);
            }

            return null;
        }
    }

}
