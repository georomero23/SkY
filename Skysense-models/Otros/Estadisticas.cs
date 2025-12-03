using Skysense_models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_models.Otros
{

    public class InversorData
    {
        public DateTime fecha { get; set; }
        public bool esEditado { get; set; } = false;
        public decimal generacion { get; set; }
    }

    public class InversorDataDiaria
    {
        public string identificador { get; set; }
        public string encabezado { get; set; }
        public string numeroSerie { get; set; }
        public InversorData[] inversorData { get; set; }
    }

    public class InversorInfo
    {
        public int idConsecutivo { get; set; }
        public string numeroSerie { get; set; }
        public DateTime? fechaInicio { get; set; }
        public DateTime? fechaFin { get; set; }
        public string identificador { get; set; }
    }

    public class TablaGeneracion
    {
        public int idInstalacion { get; set; }
        public decimal fPorcentajeDesgaste { get; set; }
        public DateOnly dtFechaInicioOperaciones { get; set; }
        public CInstalacionApigeneracionMensual[] arrValoresGarantizados { get; set; } = Array.Empty<CInstalacionApigeneracionMensual>();
        public decimal?[] arrValoresReales { get; set; } = Array.Empty<decimal?>();

    }

    [Serializable]
    public class TablaGeneracionDiaria
    {
        public string[] inversoresEncabezados { get; set; }
        public string[] inversoresIdentificadores { get; set; }
        public RenglonGeneracionDiaria[] datos { get; set; }
    }

    [Serializable]
    public class RenglonGeneracionDiaria
    {
        public int iDia { get; set; }
        public decimal?[] datosInversores { get; set; }
        public bool[] arregloEditados { get; set; }
        public decimal generacionTotal { get; set; }
        public decimal desviacionMaxima { get; set; }
    }
}
