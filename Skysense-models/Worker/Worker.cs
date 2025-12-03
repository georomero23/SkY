using Skysense_models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_models.Worker
{
    public class InstalacionWorker
    {
        public int IdInstalacion { get; set; }
        public int IdPlataforma { get; set; }
        public string IdAPI { get; set; }
        public string Nombre { get; set; }
        public DateOnly? FechaInicio { get; set; }
        public DateTime? FechaUltimaActualizacionDaemonAPI { get; set; }
        public List<CInversore> InversoresAPP { get; set; }
        public List<CInversorApiencabezado> InversoresAPI { get; set; }
    }
}
