using Skysense_models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_models.Respuestas
{
    [Serializable]
    public class ApiRespuesta<T>
    {
        public bool exito { get; set; }
        public string mensaje { get; set; } = "";
        public int codigoError { get; set; }
        public T? data { get; set; }
    }

    [Serializable]
    public class Usuario
    {
        public string id { get; set; } 
        public string name { get; set; }
        public string mail { get; set; }
        public string[] roles { get; set; }
    }

    [Serializable]
    public class PanelTabla
    {
        public int idInstalacion { get; set; }

        public int idPanel { get; set; }

        public byte idEstadoPanel { get; set; }

        public string? numeroSerie { get; set; }

        public int? idModelo { get; set; }

        public decimal? potencia { get; set; }

        public decimal? degradacionAnual { get; set; }

        public int? idProveedorSuministrado { get; set; }

        public int? idProveedorIntermediario { get; set; }

        public bool? panelFundador { get; set; }
        public string? marca { get; set; }
        public string? modelo { get; set; }
        public string? proveedorSNombre { get; set; }
        public string? proveedorSRFC { get; set; }
        public string? proveedorINombre { get; set; }
        public string? proveedorIRFC { get; set; }

    }

    public class Paginacion<T>
    {
        public int totalRegistros { get; set; }
        public int registrosPorPagina { get; set; }
        public int numeroPaginas { get; set; }
        public int paginaActual { get; set; }
        public bool masDatos { get; set; }
        public T[] registros { get; set; } = [];
    }

    public class Documento
    {
        public int idDocumento { get; set; }
        public int idInstalacion { get; set; }

        public byte tipoDocumento { get; set; }

        public string nombreDocumento { get; set; } = null!;

        public DateTime fechaModificacion { get; set; }
        public int peso { get; set; }
        public string tipoArchivo { get; set; } = null!;
    }

}
