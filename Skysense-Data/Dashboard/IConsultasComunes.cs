using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.Dashboard
{
    public interface IConsultasComunes
    {
        Skysense_models.Otros.OpcionesCatalogo[] fcObtenerOpcionesCatalago(int idCatalogo);
    }
}
