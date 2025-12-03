using AutoMapper;
using Skysense_Data.Auth.Interfaces;
using Skysense_models.DB;
using Skysense_models.Otros;
using Skysense_persistencia.Entidades;
using System.Runtime.CompilerServices;

namespace Skysense_Data.Auth.Implementation
{
    public class AuthClass : IAuth
    {
        Skysense_persistencia.Entidades.SkysenseDevContext _SkysenseDevContext;
        IMapper _mapper;

        public AuthClass(Skysense_persistencia.Entidades.SkysenseDevContext SkysenseDevContext, IMapper mapper)
        {
            this._SkysenseDevContext = SkysenseDevContext;
            this._mapper = mapper;
        }

        public bool fAcceso(CUser Usuario)
        {
            if (Usuario.UsCorreo.Length > 0 && Usuario.UsCntrsn.Length > 0)
            {
                //Tomamos al primer usuario con el correo
                var user = _SkysenseDevContext.Users
                    .Where(u => u.UsCorreo.ToLower() == Usuario.UsCorreo.ToLower())
                .FirstOrDefault();

                //Si existe el usuario y la contraseña es correcta
                return user != null && user.UsCntrsn == Usuario.UsCntrsn;
            }

            return false;
        }

        public CUser fObtenDatosDelUsuario(int iIdUsuario)
        {
            return _mapper.Map<CUser>(_SkysenseDevContext.Users
                .Where(u => u.IdUsuario == iIdUsuario)
                .SingleOrDefault());
        }
        public CUser fObtenDatosDelUsuario(string sCorreo)
        {
            return _mapper.Map<CUser>(_SkysenseDevContext.Users
                .Where(u => u.UsCorreo.ToLower() == sCorreo.ToLower())
                .SingleOrDefault());
        }
        public void mRecuperarContrasena(string psCorreo)
        {

        }
        public void mCierraSesion(CUser Usuario)
        {

        }

        public async Task<MethodResponse<int>> mInsertaNuevoUsuario(CUser usuario, string CodigoConfirmacion)
        {

            //Se inserta el usuario
            var ur = new User()
            {
                UsNombre= usuario.UsNombre,
                UsApellidos = usuario.UsApellidos,
                UsFechaAlta = DateTime.Now,
                UsCorreo = usuario.UsCorreo,
                UsCambiaCntrsn = true,
                IdRol = usuario.IdRol,
                UsCntrsn = CodigoConfirmacion,
                UsEstado = 2
            };
            this._SkysenseDevContext.Users.Add(ur);
            await this._SkysenseDevContext.SaveChangesAsync();

            return new MethodResponse<int>(true, "Éxito",0, ur.IdUsuario);
        }
    }
}
