using Skysense_models.BackgroundService;
using Skysense_models.Otros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_business.Baterias
{
    public class BateriasNegocio : IBateriasNegocio
    {
        Skysense_Data.Auth.Interfaces.IBateriasData _bateriasData;

        public BateriasNegocio(Skysense_Data.Auth.Interfaces.IBateriasData bateriasData)
        {
            this._bateriasData = bateriasData;
        }

        public async Task<bool> GuardaConfiguracionBateria(OPCBateria bateriaN)
        {
            return await this._bateriasData.GuardaConfiguracionBateria(bateriaN);
        }

        public OPCBateria? ObtenConfiguracionBateria(int idInstalacion)
        {
            return this._bateriasData.ObtenConfiguracionBateria(idInstalacion);
        }

        public TagSelectOptions[] ObtenTiposTags()
        {
            return this._bateriasData.ObtenTiposTags();
        }

        public BateriasTabla ObtenTablaBaterias(string busqueda)
        {
            return this._bateriasData.ObtenTablaBaterias(busqueda);
        }

    }
}
