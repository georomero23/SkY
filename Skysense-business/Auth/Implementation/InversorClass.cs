using Skysense_business.Auth.Interfaces;
using Skysense_models;
using Skysense_models.DB;
using Skysense_models.Otros;

namespace Skysense_business.Auth.Implementation
{
    public class InversorClass : IInversores
    {
        private readonly Skysense_Data.Auth.Interfaces.IInversores _inversores;

        public InversorClass(Skysense_Data.Auth.Interfaces.IInversores inversores)
        {
            this._inversores = inversores;
        }

        public CInversore[] mObtenInversores(int idInstalacion)
        {
            return _inversores.mObtenInversores(idInstalacion);
        }

        public CInversore? mActualizaInversor(CInversore cInstalacion)
        {
            return _inversores.mActualizaInversores(cInstalacion);
        }

        public bool mEliminaInversor(CInversore cInstalacion)
        {
            return _inversores.mEliminaInversor(cInstalacion);
        }

        public CInversore[] mObtenInversor(CInversore cInstalacion)
        {
            return _inversores.mObtenInversor(cInstalacion);
        }

        public bool mNuevosInversores(int idInstalacion, CInversore[] cInstalacion)
        {
            return _inversores.mNuevosInversores(idInstalacion, cInstalacion);
        }

        public async Task<bool> mAjustaDatosInversor(CDatosInversoresAjustado[] datos)
        {
            return await _inversores.mAjustaDatosInversor(datos);
        }

        public bool mEliminarInversor(int idInversor)
        {
            return _inversores.mEliminarInversor(idInversor);
        }

        public Skysense_models.Respuestas.Paginacion<InversorSimple> mObtenInversoresPaginados(int pagina, int registrosPorPagina, string? buscaddor)
        {
            return _inversores.mObtenInversoresPaginados(pagina, registrosPorPagina, buscaddor);
        }

    }
}
