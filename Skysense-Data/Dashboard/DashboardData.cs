using Skysense_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.Dashboard
{
    public class DashboardData : IDashboardData
    {
        private Skysense_persistencia.Entidades.SkysenseDevContext _context;
        private AutoMapper.IMapper _mapper;
        public DashboardData(Skysense_persistencia.Entidades.SkysenseDevContext context, 
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        //public CCatalogoOpcione[] fObtenCatalogo(int idCatalogoMaestro)
        //{
        //    return _mapper.Map<CCatalogoOpcione[]>(from c in _context.CatalogoOpciones
        //                                           where c.IdCatalogo == idCatalogoMaestro
        //                                           select c);
        //}

        public string fObtenCatalogoOpcion(int idCatalogoMaestro, int idCatalogoOpcion)
        {
            return (from c in _context.CatalogoOpciones
                    where c.IdCatalogo == idCatalogoMaestro &&
                    c.IdOpcion == idCatalogoOpcion
                    select c.NombreOpcion).SingleOrDefault()??"";
        }
    }
}
