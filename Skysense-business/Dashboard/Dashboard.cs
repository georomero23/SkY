using Skysense_models.DB;
using Skysense_models.Otros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_business.Dashboard
{
    public class Dashboard : IDashboard
    {
        private Skysense_Data.Dashboard.IDashboardData _dashboardData;
        private Skysense_Data.Auth.Interfaces.ICliente _clienteData;
        private Skysense_Data.Auth.Interfaces.IInsatalaciones _instalacionesData;
        public Dashboard(Skysense_Data.Dashboard.IDashboardData dashboardData, 
            Skysense_Data.Auth.Interfaces.ICliente clienteData,
            Skysense_Data.Auth.Interfaces.IInsatalaciones instalacionesData)
        {
            _dashboardData = dashboardData;
            _clienteData = clienteData;
            _instalacionesData = instalacionesData;
        }

        ClienteDashboard? IDashboard.fObtieneClienteDashboard(int idCliente)
        {
            var clienteInfo = _clienteData.mObtenClientes(new CCliente() { IdCliente = idCliente }).SingleOrDefault();
            
            if(clienteInfo == null)
            {
                return null;
            }

            ClienteDashboard cClienteDashboard = new ClienteDashboard()
            {
                cClienteInfo = clienteInfo!,
                sImagenCliente = Convert.ToBase64String(Encoding.ASCII.GetBytes(idCliente.ToString())) + ".png",
                arrInstalaciones = _instalacionesData.mObtenInstalaciones(idCliente)
                .Select(i => new InstalacionOpcion()
                {
                    idCliente = idCliente,
                    sNombreInstalacion = i.Nombre ?? "",
                    idInstalacion = i.IdInstalacion,
                    iTipoProyecto = i.TipoProyecto
                }).ToArray()
            };

            return cClienteDashboard;
        }

        InstalacionDashboard? IDashboard.fObtieneInstalacionDashboard(int idCliente, int idInstalacion)
        {
            var cInstalacion = _instalacionesData.mObtenInstalacion(idCliente, idInstalacion);

            if (cInstalacion == null)
                return null;

            var cInstalacionDashboard = new InstalacionDashboard()
            {
                idCliente = idCliente,
                idInstalacion = idInstalacion,
                sTipoTarifa = cInstalacion.sTarifa??"",
                sDireccion = cInstalacion.Ubicacion ?? "",
                sNumeroServicio = cInstalacion.CodigoProyecto ?? "",
                arrInversores = cInstalacion.Inversores.ToArray(),
                cPanel = cInstalacion.Paneles.FirstOrDefault(),
                idInstalacionAPI = cInstalacion.IdInstalacionApi??"",
                idPlataforma = cInstalacion.IdPlataforma??0,
                dtFechaInicioOperaciones = cInstalacion.InicioOperaciones ?? DateOnly.FromDateTime(DateTime.Now),
                rPU = cInstalacion.Rpu??""
            };

            return cInstalacionDashboard;
        }
    }
}
