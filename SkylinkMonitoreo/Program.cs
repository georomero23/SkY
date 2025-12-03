using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;
using SkylinkMonitoreo;
using SkylinkMonitoreo.Logica.Implementacion;
using SkylinkMonitoreo.Logica.Interfaces;
using Skysense_business.Comun;
using Skysense_Data.Auth.Implementation;
using Skysense_Data.Auth.Interfaces;
using Skysense_persistencia.Entidades;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "skylink-service.log")
    )
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Skylink Monitoreo Service";
    })
    .UseSerilog()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddTransient<IMonitoreoAPIS, MonitoreoAPIS>();
        services.AddTransient<IWorkerData, WorkerData>();
        services.AddDbContextFactory<SkysenseDevContext>(options => options.UseSqlServer(hostContext.Configuration.GetConnectionString("CadenaConexion")));
        services.AddAutoMapper(
            typeof(Automapper).Assembly
        );
    })
    .Build();

host.Run();
