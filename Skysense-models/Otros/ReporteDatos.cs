using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_models.Otros
{
    public class ReporteDatos
    {
        public int IdInstalacion { get; set; }
        public int iAnno { get; set; }
        public int iMes { get; set; }
        public string Cliente { get; set; } = "";
        public string Direccion { get; set; } = "";
        public string RPU { get; set; } = "";
        public string Periodo { get; set; } = "";
        public string NombreEnRecibo { get; set; } = "";
        public decimal CapacidadInstalada { get; set; }
        public decimal PagoSinPaneles { get; set; } = 0;
        public decimal PagoConPaneles { get; set; } = 0;
        public decimal Ahorro { get; set; } = 0;
        public decimal PorcentajeAhorro { get; set; } = 0;
        public decimal ConsumoTotal { get; set; } = 0;
        public decimal ConsumoCFE { get; set; } = 0;
        public decimal ConsumoPaneles { get; set; } = 0;
        public decimal ConsumoTotalPorcentaje { get; set; } = 0;
        public decimal ConsumoCFEPorcentaje { get; set; } = 0;
        public decimal ConsumoPanelesPorcentaje { get; set; } = 0;
        public decimal GeneracionPeriodo { get; set; } = 0;
        public decimal FijoCPaneles { get; set; } = 0;
        public decimal FijoSPaneles { get; set; } = 0;
        public decimal FijoAhorro { get; set; } = 0;
        public decimal BaseCPaneles { get; set; } = 0;
        public decimal BaseSPaneles { get; set; } = 0;
        public decimal BaseAhorro { get; set; } = 0;
        public decimal IntermediaCPaneles { get; set; } = 0;
        public decimal IntermediaSPaneles { get; set; } = 0;
        public decimal IntermediaAhorro { get; set; } = 0;
        public decimal PuntaCPaneles { get; set; } = 0;
        public decimal PuntaSPaneles { get; set; } = 0;
        public decimal PuntaAhorro { get; set; } = 0;
        public decimal TransmisionCPaneles { get; set; } = 0;
        public decimal TransmisionSPaneles { get; set; } = 0;
        public decimal TransmisionAhorro { get; set; } = 0;
        public decimal CENACECPaneles { get; set; } = 0;
        public decimal CENACESPaneles { get; set; } = 0;
        public decimal CENACEAhorro { get; set; } = 0;
        public decimal SCNMEMCPaneles { get; set; } = 0;
        public decimal SCNMEMSPaneles { get; set; } = 0;
        public decimal SCNMEMAhorro { get; set; } = 0;
        public decimal DistribucionCPaneles { get; set; } = 0;
        public decimal DistribucionSPaneles { get; set; } = 0;
        public decimal DistribucionAhorro { get; set; } = 0;
        public decimal CapacidadCPaneles { get; set; } = 0;
        public decimal CapacidadSPaneles { get; set; } = 0;
        public decimal CapacidadAhorro { get; set; } = 0;
        public decimal EnergiaCPaneles { get; set; } = 0;
        public decimal EnergiaSPaneles { get; set; } = 0;
        public decimal EnergiaAhorro { get; set; } = 0;
        public decimal FactorPotenciaCPaneles { get; set; } = 0;
        public decimal FactorPotenciaSPaneles { get; set; } = 0;
        public decimal FactorPotenciaAhorro { get; set; } = 0;
        public decimal SubtotalCPaneles { get; set; } = 0;
        public decimal SubtotalSPaneles { get; set; } = 0;
        public decimal SubtotalAhorro { get; set; } = 0;
        public decimal DAPCPaneles { get; set; } = 0;
        public decimal DAPSPaneles { get; set; } = 0;
        public decimal DAPAhorro { get; set; } = 0;
        public decimal IVACPaneles { get; set; } = 0;
        public decimal IVASPaneles { get; set; } = 0;
        public decimal IVAhorro { get; set; } = 0;
        public decimal TotalCPaneles { get; set; } = 0;
        public decimal TotalSPaneles { get; set; } = 0;
        public decimal TotalAhorro { get; set; } = 0;
        public decimal AhorroAcumuladoSIva { get; set; } = 0;
        public string AporteArboles { get; set; } = "0 árboles.";
        public int AporteCO2 { get; set; } = 0;
        public string AhorroAcumuladoDesde { get; set; } = "";
        public decimal PorcentajeCumplimiento { get; set; } = 0;
        public decimal[] GeneracionDiaria { get; set; } = [];
        public decimal[] HistoricoGenPVEsteAnnio { get; set; } = [];
        public decimal[] HistoricoGenPVAnnioAnterior { get; set; } = [];
        public decimal?[] HistoricoConsumo { get; set; } = [];
        public decimal?[][] HistoricoFacturas { get; set; } = [];
    }
}
