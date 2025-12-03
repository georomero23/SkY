using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.Dashboard
{
    public class ConsultasComunes:IConsultasComunes
    {
        private Skysense_persistencia.Entidades.SkysenseDevContext _DBContext;

        public ConsultasComunes(Skysense_persistencia.Entidades.SkysenseDevContext DBContext) { 
            this._DBContext = DBContext;
        }

        public Skysense_models.Otros.OpcionesCatalogo[] fcObtenerOpcionesCatalago(int idCatalogo)
        {
            return (from c in _DBContext.CatalogoOpciones
                    where c.IdCatalogo == idCatalogo
                    select new Skysense_models.Otros.OpcionesCatalogo()
                    {
                        iIdOpcion = c.IdOpcion,
                        sOpcion = c.NombreOpcion ?? ""
                    }).ToArray();
        }
    }
}
