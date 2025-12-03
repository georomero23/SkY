using Skysense_models;
using Skysense_models.DB;
using Skysense_models.Otros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_business.Auth.Interfaces
{
    public interface IInversores
    {
        public CInversore[] mObtenInversores(int idInstalacion);
        public CInversore? mActualizaInversor(CInversore cInstalacion);
        public bool mEliminaInversor(CInversore cInstalacion);
        public CInversore[] mObtenInversor(CInversore cInstalacion);
        public bool mNuevosInversores(int idInstalacion, CInversore[] cInstalacion);
        public Task<bool> mAjustaDatosInversor(CDatosInversoresAjustado[] datos);
        bool mEliminarInversor(int idInversor);
        Skysense_models.Respuestas.Paginacion<InversorSimple> mObtenInversoresPaginados(int pagina, int registrosPorPagina, string? buscaddor);
    }
}
