using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_persistencia.EntidadesBateriasMonitoreo
{
    public class MedicionesBess
    {
        public int IdMedicion { get; set; }
        public int IdTag { get; set; }
        public DateTime FechaMedicion { get; set; }
        public int ValorMedicion { get; set; }
    }
}
