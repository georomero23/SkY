using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_business.Dashboard
{
    public  interface IDashboard
    {
        public Skysense_models.Otros.ClienteDashboard? fObtieneClienteDashboard(int idCliente);
        public Skysense_models.Otros.InstalacionDashboard? fObtieneInstalacionDashboard(int idCliente, int idInstalacion);
    }
}
