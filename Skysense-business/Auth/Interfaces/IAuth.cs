using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skysense_models.DB;
using Skysense_models.Otros;

namespace Skysense_business.Auth.Interfaces
{
    public interface IAuth
    {
        public CUser? fAcceso(CUser Usuario);
        public CUser? mObtenInfoUsuario(string? idUsuario);
        public Task<MethodResponse<int>> mInsertaNuevoUsuario(CUser usuario);
        Task<MethodResponse<bool>> mCambiaContrasena(int idUsuario, string sAnterior, string sNueva);
    }
}
