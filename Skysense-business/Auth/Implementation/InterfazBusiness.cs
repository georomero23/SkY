using Skysense_business.Auth.Interfaces;
using Skysense_business.Comun;
using Skysense_models.DB;
using Skysense_models.Otros;
using Skysense_models.Peticiones;

namespace Skysense_business.Auth.Implementation
{
    public class InterfazBusiness : IInterfazBusiness
    {
        private readonly Skysense_Data.Auth.Interfaces.IInterfazData _interf;

        public InterfazBusiness(Skysense_Data.Auth.Interfaces.IInterfazData interf)
        {
            this._interf = interf;
        }

        public bool mActualizaCatalogoMaestro(CCatalogoOpcione opcionAModificar)
        {
            return this._interf.mActualizaCatalogoMaestro(opcionAModificar);
        }

        public CCatalogoMaestro[] mObtenCatalogoMaestro(int[] idCatalogos)
        {
            return this._interf.mObtenCatalogoMaestro(idCatalogos);
        }

        public CCatalogoMaestro[] mObtenCatalogosMaestros()
        {
            return this._interf.mObtenCatalogosMaestros();
        }

        public bool mEliminaOpcionCatalogo(CCatalogoOpcione opcionAEliminar)
        {
            return this._interf.mEliminaOpcionCatalogo(opcionAEliminar);
        }


        public CGrupo[] mObtenGrupos()
        {
            return this._interf.mObtenGrupos();
        }
        public bool mEliminaGrupo(int opcionAEliminar)
        {
            return this._interf.mEliminaGrupo(opcionAEliminar);
        }
        public bool mActualizaGrupo(CGrupo opcionAModificar)
        {
            return this._interf.mActualizaGrupo(opcionAModificar);
        }


        public string[] mObtenRolesSistema()
        {
            return this._interf.mObtenRolesSistema();
        }
        public async Task<bool> mActualizaUsuario(UserTabla usuarioAModificar)
        {
            return await this._interf.mActualizaUsuario(usuarioAModificar);
        }
        public UserTabla[] mObtenUsuarios()
        {
            return this._interf.mObtenUsuarios();
        }

        public CTarifasDivisione[] mObtenTarifasDivisiones(int mes, int annio, int IdZona, int? idTarifa = null)
        {
            return this._interf.mObtenTarifasDivisiones(mes, annio, IdZona, idTarifa);
        }

        public bool mGuardaTarifasDivisiones(int iMes, int iAnno, int idZona, CTarifasDivisione[] tarifas)
        {
            return this._interf.mGuardaTarifasDivisiones(iMes, iAnno, idZona, tarifas);
        }


        public DateOnly[] mObtenDiasFestivos(int iAnno)
        {
            return this._interf.mObtenDiasFestivos(iAnno);
        }
        public bool mGuardaDiasFestivos(int iAnno,DateOnly[] dias)
        {
            return this._interf.mGuardaDiasFestivos(iAnno,dias);
        }


    }
}
