using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Skysense_Data.Auth.Implementation;
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
    public interface IAPIWorker
    {
        public Task<bool> ActualizaGeneracionDiaria(IEnumerable<CInversorApigeneracion> generacionInversores);

        public Task<CInversorApiencabezado[]> DarDeAltaInversoresAPI(List<CInversorApiencabezado> inversores);

        public Task mActualizaGeneracionMensualParametros(int idInstalacion, int month, int year);

        public Task<InstalacionWorker[]> ObtenerInstalaciones(int idPlataforma);

        public Task<InstalacionWorker> ObtenerInstalacion(int idInstalacion);

    }
}
