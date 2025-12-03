using Microsoft.Extensions.Logging;
using Skysense_Data.BackgroundService;
using Skysense_models.BackgroundService;
using Skysense_models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_business.BackgroundService
{
    public class OPCNegocio : IOPCNegocio
    {
        private IOPCData _opcData;
        private ILogger<OPCNegocio> _logger;

        public OPCNegocio(IOPCData opcData, ILogger<OPCNegocio> logger)
        {
            _opcData = opcData;
            _logger = logger;
        }

        public async Task<OPCBateria[]> ObtenConfiguracionesOPC(params int[] idBaterias)
        {
            return await _opcData.ObtenConfiguracionesOPC(idBaterias);
        }

        public async Task<OPCBateria[]> ObtenMediciones(DateTime? startDate, DateTime? endDate, params int[] IdBaterias)
        {
            return await _opcData.ObtenMediciones(startDate, endDate, IdBaterias);
        }

        public async Task GuardarMediciones(TagMedicion[] mediciones, string? CadenaConexion)
        {
            if (CadenaConexion == null)
            {
                _logger.LogWarning("No se proporcionó ninguna cadena de conexión.");
                return;
            }

            await _opcData.GuardarMediciones(mediciones, CadenaConexion!);
        }

        public async Task CambiarEstado(Entidad entidad, int idEntidad, byte NuevoEstado, string? MensajeEstado)
        {
            await _opcData.CambiarEstado(entidad, idEntidad, NuevoEstado, MensajeEstado);
        }
    }
}
