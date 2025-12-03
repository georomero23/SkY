using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_models.Otros
{
    public class InversorSimple
    {
        public int idInversor { get; set; }
        public string numeroSerie { get; set; } = "";
        public string modelo { get; set; } = "";
        public string marca { get; set; } = "";
        public decimal potencia { get; set; } = 0;
        public string instalacion { get; set; } = "";
        public int idInstalacion { get; set; } = 0;
        public int idCliente { get; set; } = 0;
    }

    public class PanelSimple
    {
        public int idPanel { get; set; }
        public string numeroSerie { get; set; } = "";
        public string modelo { get; set; } = "";
        public string marca { get; set; } = "";
        public decimal potencia { get; set; } = 0;
        public string instalacion { get; set; } = "";
        public int idInstalacion { get; set; } = 0;
        public int idCliente { get; set; } = 0;
    }
}
