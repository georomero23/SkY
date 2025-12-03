using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skysense_models.DB;
using Skysense_models.Respuestas;

namespace Skysense_business.Auth.Interfaces
{
    public interface ICLiente
    {
        public Paginacion<CCliente> mObtenClientes(int idPagina, int cuantosRegistros, string? busqueda);
        public CCliente? mActualizaCliente(CCliente cCliente);
        public bool mEliminaCliente(CCliente cCliente);
        public CCliente[] mObtenClientes(CCliente cCliente);
        public CCliente mNuevoCliente(CCliente cCliente);
        public Task<string> mFusionaClientes(int idCliente, string? rfc);
    }
}
