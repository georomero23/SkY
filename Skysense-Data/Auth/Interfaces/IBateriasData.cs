using Skysense_models.BackgroundService;
using Skysense_models.Otros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.Auth.Interfaces
{
    public interface IBateriasData
    {
        public TagSelectOptions[] ObtenTiposTags();

        public OPCBateria? ObtenConfiguracionBateria(int idInstalacion);

        public Task<bool> GuardaConfiguracionBateria(OPCBateria bateriaN);

        BateriasTabla ObtenTablaBaterias(string busqueda);
    }
}
