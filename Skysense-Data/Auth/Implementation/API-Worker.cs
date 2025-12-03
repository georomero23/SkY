using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Skysense_Data.Auth.Interfaces;
using Skysense_models.DB;
using Skysense_models.Worker;
using Skysense_persistencia.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.Auth.Implementation
{
    public class APIWorker : IAPIWorker
    {
        private readonly IWorkerData _workerData;
        private readonly SkysenseDevContext _context;

        public APIWorker(IWorkerData workerData, SkysenseDevContext context)
        {
            _workerData = workerData;
            _context = context;
        }

       public async Task<bool> ActualizaGeneracionDiaria(IEnumerable<CInversorApigeneracion> generacionInversores)
       {
           return await _workerData.ActualizaGeneracionDiaria(_context, generacionInversores);
       }

       public async Task<CInversorApiencabezado[]> DarDeAltaInversoresAPI(List<CInversorApiencabezado> inversores)
       {
           return await _workerData.DarDeAltaInversoresAPI(_context, inversores);
       }

       public async Task mActualizaGeneracionMensualParametros(int idInstalacion, int month, int year)
       {
           await _workerData.mActualizaGeneracionMensualParametros(_context, idInstalacion, month, year);
       }

       public async Task<InstalacionWorker[]> ObtenerInstalaciones(int idPlataforma)
       {
           return await _workerData.ObtenerInstalaciones(_context, idPlataforma);
       }

       public async Task<InstalacionWorker> ObtenerInstalacion(int idInstalacion)
       {
           return await _workerData.ObtenerInstalacion(_context, idInstalacion);
       }
    }
}
