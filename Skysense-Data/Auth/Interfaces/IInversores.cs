using Skysense_models;
using Skysense_models.DB;
using Skysense_models.Otros;
using Skysense_models.Respuestas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.Auth.Interfaces
{
    public interface IInversores
    {
        public CInversore[] mObtenInversores(int idInstalacion);
        public CInversore? mActualizaInversores(CInversore cInstalacion);
        public bool mEliminaInversor(CInversore cInstalacion);
        public CInversore[] mObtenInversor(CInversore cInstalacion);
        public bool mNuevosInversores(int idInstalacion, CInversore[] cInstalacion);
        public Task<bool> mAjustaDatosInversor(CDatosInversoresAjustado[] datos);
        bool mEliminarInversor(int idInversor);
        Paginacion<InversorSimple> mObtenInversoresPaginados(int pagina, int registrosPorPagina, string? buscaddor);
    }
}
