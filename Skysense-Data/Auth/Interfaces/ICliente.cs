using Skysense_models.DB;
using Skysense_models.Respuestas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.Auth.Interfaces
{
    public interface ICliente
    {
        public Paginacion<CCliente> mObtenClientes(int idPagina, int cuantosRegistros, string? busqueda);
        public CCliente? mActualizaCliente(CCliente cCliente);
        public bool mEliminaCliente(CCliente cCliente);
        public CCliente[] mObtenClientes(CCliente cCliente);
        public CCliente mNuevoCliente(CCliente cCliente);
        Task<string> mFusionaClientes(int idCliente, string rfc);
    }
}
