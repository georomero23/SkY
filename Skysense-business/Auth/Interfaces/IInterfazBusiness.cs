using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skysense_models.DB;
using Skysense_models.Otros;
using Skysense_models.Peticiones;

namespace Skysense_business.Auth.Interfaces
{
    public interface IInterfazBusiness
    {
        CCatalogoMaestro[] mObtenCatalogosMaestros();
        CCatalogoMaestro[] mObtenCatalogoMaestro(int[] idCatalogos);
        bool mActualizaCatalogoMaestro(CCatalogoOpcione opcionAModificar);
        bool mEliminaOpcionCatalogo(CCatalogoOpcione opcionAEliminar);
        CGrupo[] mObtenGrupos();
        bool mEliminaGrupo(int opcionAEliminar);
        bool mActualizaGrupo(CGrupo opcionAModificar);
        string[] mObtenRolesSistema();
        Task<bool> mActualizaUsuario(UserTabla usuarioAModificar);
        UserTabla[] mObtenUsuarios();
        CTarifasDivisione[] mObtenTarifasDivisiones(int mes, int annio, int IdZona, int? idTarifa = null);
        bool mGuardaTarifasDivisiones(int iMes, int iAnno, int idZona, CTarifasDivisione[] tarifas);
        DateOnly[] mObtenDiasFestivos(int iAnno);
        bool mGuardaDiasFestivos(int iAnno,DateOnly[] dias);
    }
}
