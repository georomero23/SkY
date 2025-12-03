using Skysense_models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_models.Otros
{
    public class ClienteDashboard
    {
        public CCliente cClienteInfo {  get; set; }
        public InstalacionOpcion[] arrInstalaciones { get; set; }
        public string sImagenCliente { get; set; }
    }
    public class InstalacionDashboard
    {
        public int idCliente { get; set; }
        public int idInstalacion { get; set; }
        public string sNumeroServicio { get; set; }
        public string idInstalacionAPI { get; set; }
        public int idPlataforma { get; set; }
        public string sTipoTarifa { get; set; }
        public string sDireccion { get; set; }
        public DB.CPanele? cPanel { get; set; }
        public DB.CInversore[] arrInversores { get; set; }
        public DateOnly dtFechaInicioOperaciones { get; set; }
        public string rPU { get; set; }
    }

    public class InstalacionOpcion
    {
        public int idCliente { get; set; }
        public int idInstalacion { get; set; }
        public string sNombreInstalacion { get; set; }
        public int iTipoProyecto { get; set; }
    }
}
