using AutoMapper;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Skysense_app.Server;
using Skysense_app.Server.BackgroundServices;
using Skysense_app.Server.WebSocket;
using Skysense_business.Auth.Implementation;
using Skysense_business.Auth.Interfaces;
using Skysense_business.BackgroundService;
using Skysense_business.Baterias;
using Skysense_business.Comun;
using Skysense_business.Dashboard;
using Skysense_Data.Auth.Interfaces;
using Skysense_Data.BackgroundService;
using Skysense_models;
using Skysense_persistencia.Entidades;
using Skysense_persistencia.EntidadesBESS;
using Skysense_persistencia.Identity;
using System.Threading.Tasks;
using System.Transactions;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //builder.Services.AddMemoryCache();
        //builder.Services.AddSession(options =>
        //{
        //    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
        //    options.Cookie.HttpOnly = true; // Ensures the session cookie is accessible only by the server
        //    options.Cookie.IsEssential = true; // Required for GDPR compliance
        //});

        // Add services to the container.
        builder.Services.AddControllers();

        //builder.Services.AddAuthentication("cookie")
        //                .AddCookie("cookie", options =>
        //                {
        //                    options.Cookie.Name = "AuthCookie";
        //                    options.LoginPath = "/#/login";
        //                    options.AccessDeniedPath = "/#/404";
        //                    options.Events.OnRedirectToLogin = context =>
        //                    {
        //                        if (context.Request.Path.StartsWithSegments("/api")) // Check if it's an API request
        //                        {
        //                            context.Response.StatusCode = 401;
        //                        }
        //                        else
        //                        {
        //                            context.Response.Redirect(options.LoginPath); // Redirect for non-API requests
        //                        }
        //                        return Task.CompletedTask;
        //                    };
        //                });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<Skysense_business.Auth.Interfaces.IAuth, AuthClass>()
                        .AddScoped<Skysense_business.Auth.Interfaces.ICLiente, ClienteClass>()
                        .AddScoped<Skysense_business.Auth.Interfaces.IInversores, InversorClass>()
                        .AddScoped<Skysense_business.Auth.Interfaces.IInstalaciones, InstalacionClass>()
                        .AddScoped<Skysense_business.Dashboard.IDashboard, Dashboard>()
                        .AddScoped<IInterfazBusiness, InterfazBusiness>()
                        .AddScoped<IBateriasNegocio, BateriasNegocio>()
                        .AddSingleton<IOPCNegocio, OPCNegocio>();

        builder.Services.AddScoped<Skysense_Data.Auth.Interfaces.IAuth, Skysense_Data.Auth.Implementation.AuthClass>();
        builder.Services.AddScoped<Skysense_Data.Auth.Interfaces.ICliente, Skysense_Data.Auth.Implementation.ClienteClass>();
        builder.Services.AddScoped<Skysense_Data.Auth.Interfaces.IInversores, Skysense_Data.Auth.Implementation.InversorClass>();
        builder.Services.AddScoped<Skysense_Data.Auth.Interfaces.IInsatalaciones, Skysense_Data.Auth.Implementation.InstalacionClass>();
        builder.Services.AddScoped<Skysense_Data.Dashboard.IDashboardData, Skysense_Data.Dashboard.DashboardData>();
        builder.Services.AddScoped<Skysense_Data.Dashboard.IConsultasComunes, Skysense_Data.Dashboard.ConsultasComunes>();
        builder.Services.AddScoped<IInterfazData, Skysense_Data.Auth.Implementation.InterfazData>();
        builder.Services.AddScoped<IWorkerData, Skysense_Data.Auth.Implementation.WorkerData>();
        builder.Services.AddScoped<IAPIWorker, Skysense_Data.Auth.Implementation.APIWorker>();
        builder.Services.AddScoped<IBateriasData, Skysense_Data.Auth.Implementation.BateriasData>();
        builder.Services.AddSingleton<IOPCData, Skysense_Data.BackgroundService.OPCData>();

        builder.Services.AddDbContext<SkysenseDevContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaConexion")));
        builder.Services.AddDbContext<SkysenseBateriasContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaConexionBaterias")));

        //var mapperConfig = new MapperConfiguration(mc =>
        //{
        //    mc.AddProfile(new Automapper());
        //});
        //IMapper mapper = mapperConfig.CreateMapper();

        //builder.Services.AddSingleton(mapper);

        builder.Services.AddAutoMapper(
            typeof(Automapper).Assembly
        );

        #region Configuracion del BackgroundService

        //builder.Services.AddHostedService<OPCDataPolling>();
        //builder.Services.AddSingleton<IOPCDataPolling, OPCDataPolling>();

        #endregion

        builder.Services.AddSignalR();
        
        builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaConexion")));
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);
        //builder.Services.AddIdentityApiEndpoints<IdentityUser>();
        builder.Services.AddIdentityCore<IdentityUser>()
                        .AddRoles<IdentityRole>()
                        .AddEntityFrameworkStores<AuthDbContext>()
                        .AddApiEndpoints();
        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Default SignIn settings.
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });

        //Se configura el mail
        builder.Services.AddTransient<IEmailSender, EnviadorDeCorreo>();

        //Se configura el logger
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext().CreateLogger());

        var app = builder.Build();

        //Creación y verificación de datos iniciales de base de datos
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Administrador", "Operador Nivel 01", "Operador Nivel 02", "Operador Nivel 03", "Cliente" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        app.MapGroup("api/Identity").MapIdentityApi<IdentityUser>();
        //app.UseRouting();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        //app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.MapPost("api/Identity/logout", async (SignInManager<IdentityUser> signInManager,
            [FromBody] object empty) =>
                {
                    if (empty != null)
                    {
                        await signInManager.SignOutAsync();
                        return Results.Ok();
                    }
                    return Results.Unauthorized();
                })
        .RequireAuthorization();

        app.MapHub<WebSocketHub>("api/skyhub").RequireAuthorization();
        app.Run();
    }


}
