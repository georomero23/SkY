using Skysense_business.Auth.Interfaces;
using Skysense_models.DB;
using Skysense_models.Errors;
using Skysense_models.Respuestas;

namespace Skysense_business.Auth.Implementation
{
    public class ClienteClass : ICLiente
    {
        private readonly Skysense_Data.Auth.Interfaces.ICliente _cliente;

        public ClienteClass(Skysense_Data.Auth.Interfaces.ICliente cliente)
        {
            this._cliente = cliente;
        }

        public Paginacion<CCliente> mObtenClientes(int idPagina, int cuantosRegistros, string? busqueda)
        {
            return _cliente.mObtenClientes(idPagina, cuantosRegistros, busqueda);
        }

        public CCliente? mActualizaCliente(CCliente cCliente)
        {
            return _cliente.mActualizaCliente(cCliente);
        }

        public bool mEliminaCliente(CCliente cCliente)
        {
            return _cliente.mEliminaCliente(cCliente);
        }

        public CCliente[] mObtenClientes(CCliente cCliente)
        {
            return _cliente.mObtenClientes(cCliente);
        }

        public CCliente mNuevoCliente(CCliente cCliente)
        {
            return _cliente.mNuevoCliente(cCliente);
        }

        public async Task<string> mFusionaClientes(int idCliente, string? rfc)
        {
            if (idCliente <= 0 || string.IsNullOrEmpty(rfc))
            {
                throw new ErrorAlCliente("Parámetros no válidos.");
            }
            else
            {
                return await this._cliente.mFusionaClientes(idCliente, rfc);
            }
        }

    }
}
