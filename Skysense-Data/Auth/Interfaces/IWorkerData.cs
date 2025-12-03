using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Skysense_models.DB;
using Skysense_models.Worker;
using Skysense_persistencia.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.Auth.Interfaces
{
    public interface IWorkerData
    {
        //Obtener Instalaciones con IdPlataforma 5,6,3 y que tengan idAPI no nulo ni vacio
        //Debe de venir: InstalacionId, IdPlataforma, IdAPI, Inversores, InversoresAPI
        public Task<InstalacionWorker[]> ObtenerInstalaciones(SkysenseDevContext _context, int idPlataforma);

        public Task<InstalacionWorker> ObtenerInstalacion(SkysenseDevContext _context, int idInstalacion);

        //Actualizar la tabla de GeneracionDiaria (del dia actual o del anterior)
        public Task<bool> ActualizaGeneracionDiaria(SkysenseDevContext _context, IEnumerable<CInversorApigeneracion> generacionInversores);

        //Alta de inversores API
        public Task<CInversorApiencabezado[]> DarDeAltaInversoresAPI(SkysenseDevContext _context, List<CInversorApiencabezado> inversores);
        public Task mActualizaGeneracionMensualParametros(SkysenseDevContext skysenseDevContext, int idInstalacion, int month, int year);
    }
}
