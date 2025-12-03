using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_models.Peticiones
{
    public class DatoAjustado
    {
        public int idInstalacion { get; set; }
        public string identificadorInversor { get; set; } = "";
        public DateTime fecha { get; set; }
        public decimal valorOriginal { get; set; }
        public decimal valorNuevo { get; set; }
    }

    public class PanelInfoPeticion
    {
        public string numeroSerie { get; set; }
        public string? marca { get; set; }
        public string? modelo { get; set; }
        public decimal potencia { get; set; }
        public decimal degradacionAnual { get; set; }
        public string? proveedorSuministrador { get; set; }
        public string? proveedorSuministradorRFC { get; set; }
        public string? proveedorIntermediario { get; set; }
        public string? proveedorIntermediarioRFC { get; set; }
        public bool esFundador { get; set; }
        public string? numeroSerieDanado { get; set; }
    }

    public class UserTabla
    {
        public string id { get; set; } = "";
        public string name { get; set; } = "";
        public string mail { get; set; } = "";
        public bool bloqueado { get; set; } = false;
        public bool? modificaBloqueo { get; set; } = null;
        public string rol { get; set; } = "";
    }
}
