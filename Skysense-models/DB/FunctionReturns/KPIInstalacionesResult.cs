using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_models.DB.FunctionReturns
{
    public class KPIInstalacionesResult
    {
        public int IdInstalacion { get; set; }
        public string Nombre { get; set; }
        public string IdCliente { get; set; }
        public short? IdGrupo { get; set; }
        public short? Anno { get; set; }
        public decimal? GeneracionReal { get; set; }
        public decimal? GeneracionGarantizada { get; set; }
        public decimal? Porcentaje { get; set; }
    }
}
