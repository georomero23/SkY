using Skysense_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.Dashboard
{
    public interface IDashboardData
    {
        //CCatalogoOpcione[] fObtenCatalogo(int idCatalogoMaestro);
        string fObtenCatalogoOpcion(int idCatalogoMaestro, int idCatalogoOpcion);
    }
}
