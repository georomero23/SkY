using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylinkMonitoreo.Logica.Interfaces
{
    public  interface IMonitoreoAPIS
    {
        Task<bool> MonitoreoDePlataformas(bool primeroDelDia);
    }
}
