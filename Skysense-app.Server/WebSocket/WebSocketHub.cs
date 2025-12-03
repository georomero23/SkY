using Microsoft.AspNetCore.SignalR;
using Skysense_models.BackgroundService;

namespace Skysense_app.Server.WebSocket
{
    public class WebSocketHub: Hub
    {


        #region Métodos Entrada
        public async Task MonitorearBaterias(int idBaterias)
        {
            //foreach (int id in idBaterias) {
                await Groups.AddToGroupAsync(this.Context.ConnectionId, "MonitoreoBateriaID" + idBaterias);
            //}
        }

        public async Task MonitorearBateria(int idBateria)
        {
            await Groups.AddToGroupAsync(this.Context.ConnectionId, "MonitoreoBateriaID" + idBateria);
        }
        #endregion

        #region Métodos Salida
        public async Task EnviaDatosRealTime(OPCBateria[] datos)
        {
            foreach (var bateria in datos)
            {
                await Clients.Group("MonitoreoBateriaID" + bateria.IdBateria.ToString()).SendCoreAsync("MonitoreoBateria", new object[] { bateria });
            }
        }
        #endregion
    }
}
