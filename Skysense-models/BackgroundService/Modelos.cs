using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_models.BackgroundService
{
    public class OPCBateria
    {
        public int IdInstalacion { get; set; }
        public int IdBateria { get; set; }
        public string? NombreInstalacion { get; set; }
        public string? Url { get; set; }
        public bool Monitorear { get; set; }
        public string? CadenaConexion { get; set; }
        public TagBateria[]? TagsBateria { get; set; }
    }

    public class TagBateria
    {
        public int IdTag { get; set; }
        public int IdParametro { get; set; }
        public TagMedicion[]? Mediciones { get; set; }
        public string? Etiqueta { get; set; }
        public string? Unidad { get; set; }
        public string? NombreAMostrar { get; set; }
        public bool Monitorear { get; set; }
        public bool Graficable { get; set; }
        public byte? IdGrafica { get; set; }
        public bool SoloLectura { get; set; }
    }

    public class TagMedicion
    {
        public int IdTag { get; set; }
        public decimal Medicion { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class TagSelectOptions
    {
        public string Value { get; set; } = "";
        public string Label { get; set; } = "";
        public bool Graficable { get; set; }
        public bool SoloLectura { get; set; }

    }

    public class BateriasTabla
    {
        public DateTime UltimaActualizacion { get; set; }

        public BateriaRegistro[] Baterias { get; set; } = [];
    }

    public class BateriaRegistro
    {
        public int IdInstalacion { get; set; }
        public string Estatus { get; set; } = "";
        public byte IdEstatus { get; set; }
        public string NombreInstalacion { get; set; } = "";
        public int Alertas { get; set; }
        public int TagsTotales { get; set; }
        public int TagsOnline { get; set; }
        public string MensajeUltimo { get; set; } = "";
    }
}
