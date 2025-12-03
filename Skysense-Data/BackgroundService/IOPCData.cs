using Skysense_models.BackgroundService;
using Skysense_models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.BackgroundService
{
    public interface IOPCData
    {
        public Task<OPCBateria[]> ObtenConfiguracionesOPC(params int[] idBaterias);

        /// <summary>
        /// Obtiene las mediciones de las baterias.
        /// </summary>
        /// <param name="startDate">Fecha de inicio de búsqueda</param>
        /// <param name="endDate">Fecha de fin de búsqueda</param>
        /// <param name="IdBaterias">Identificadores de las baterías a buscar. No poner nada si se requiere buscar todas.</param>
        /// <returns></returns>
        public Task<OPCBateria[]> ObtenMediciones(DateTime? startDate, DateTime? endDate, params int[] IdBaterias);

        public Task GuardarMediciones(TagMedicion[] mediciones, string CadenaConexion);

        public Task CambiarEstado(Entidad entidad, int idEntidad, byte NuevoEstado, string? MensajeEstado);
    }
}
