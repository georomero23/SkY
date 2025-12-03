namespace Skysense_app.Server.BackgroundServices
{
    public interface IOPCDataPolling
    {
        Task ActualizaMonitoreo(int idBateria, CancellationToken ct = default);
    }
}
