using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skysense_models.DB;
using Skysense_models.Otros;

namespace Skysense_Data.Auth.Interfaces
{
    public interface IAuth
    {
        public bool fAcceso(CUser Usuario);
        public CUser fObtenDatosDelUsuario(int iIdUsuario);
        public CUser fObtenDatosDelUsuario(string sCorreo);
        public void mRecuperarContrasena(string psCorreo);
        public void mCierraSesion(CUser Usuario);
        Task<MethodResponse<int>> mInsertaNuevoUsuario(CUser usuario, string CodigoConfirmacion);
    }
}
