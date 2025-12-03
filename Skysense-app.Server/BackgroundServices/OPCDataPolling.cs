using Azure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;
using Skysense_business.BackgroundService;
using Skysense_models.BackgroundService;
using Skysense_models.Enums;
using Skysense_persistencia.EntidadesBESS;
using System.Collections.Concurrent;
using System.Formats.Asn1;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Skysense_app.Server.BackgroundServices
{
    public class OPCDataPolling : BackgroundService, IOPCDataPolling
    {
        private const int _samplingInterval = 1000;
        private const int _publishingInterval = 5000;

        private IOPCNegocio _opcNegocio;
        private const int _milisegundosEspera = 1000;
        private IHubContext<WebSocket.WebSocketHub> _hub;
        private ILogger<OPCDataPolling> _logger;
        //private ConcurrentBag<BateriaSuscripcion> _subscriptions = new ConcurrentBag<BateriaSuscripcion>();
        private ConcurrentBag<OPC_Bateria> bateriasMonitoreando = new ConcurrentBag<OPC_Bateria> ();

        public OPCDataPolling(IOPCNegocio opcNegocio, IHubContext<WebSocket.WebSocketHub> hub, ILogger<OPCDataPolling> logger)
        {
            _opcNegocio = opcNegocio;
            _hub = hub;
            _logger = logger;
        }

        protected async override Task? ExecuteAsync(CancellationToken ct)
        {
            try
            {
                //Primero obtenemos los datos que tenemos configurados
                var configuracion = await _opcNegocio.ObtenConfiguracionesOPC();

                foreach (var config in configuracion)
                {
                    var bat = new OPC_Bateria(config.IdBateria, new OPC_BateriaConfiguracion()
                    {
                        UrlEMS = "opc.tcp://" + config.Url,
                        CadenaConexion = config.CadenaConexion,
                        Monitorear = config.Monitorear
                    }, this);

                    foreach (var tag in config.TagsBateria ?? [])
                    {
                        bat.AgregaVariable(tag.IdTag, new OPC_VariableConfiguracion()
                        {
                            Tag = tag.Etiqueta,
                            Unidad = tag.Unidad,
                            Monitorear = tag.Monitorear,
                            EsSoloLectura = tag.SoloLectura,
                            EsGraficable = tag.Graficable,
                            IdAgrupacionGrafica = tag.IdGrafica,
                            NombreAMostrar = tag.NombreAMostrar ?? ""
                        });
                    }

                    bateriasMonitoreando.Add(bat);

                    await bat.EmpezarAMonitorear(default);
                }



            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error en ExecuteAsync de OPCDataPolling");
            }

            //return Task.CompletedTask;
        }

        //private async Task EmpezarMonitoreo(OPCBateria bateria, CancellationToken ct)
        //{
        //    // Configuración básica sin certificados
        //    #region Certificados
        //    var config = new ApplicationConfiguration()
        //    {
        //        ApplicationName = "OpcUaConsoleSubscriber",
        //        ApplicationType = ApplicationType.Client,
        //        SecurityConfiguration = new SecurityConfiguration
        //        {
        //            ApplicationCertificate = new CertificateIdentifier(),
        //            AutoAcceptUntrustedCertificates = true,
        //            AddAppCertToTrustedStore = false,
        //            TrustedIssuerCertificates = new CertificateTrustList() { StoreType = "Directory", StorePath = "%CommonApplicationData%\\OPC Foundation\\pki\\issuer" },
        //            TrustedPeerCertificates = new CertificateTrustList() { StoreType = "Directory", StorePath = "%CommonApplicationData%\\OPC Foundation\\pki\\trusted" },
        //        },
        //        TransportConfigurations = new TransportConfigurationCollection(),
        //        TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
        //        ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
        //    };
        //    await config.ValidateAsync(ApplicationType.Client, ct);
        //    #endregion


        //    var endpointURL = "opc.tcp://" + bateria.Url;

        //    if (!System.Uri.TryCreate(endpointURL, UriKind.Absolute, out Uri? result))
        //    {
        //        _logger?.LogWarning("URI inválida para la batería {IdInstalacion}: {Url}", bateria.IdBateria, endpointURL);
        //        await _opcNegocio.CambiarEstado(Skysense_models.Enums.Entidad.Bateria, bateria.IdBateria, 5, "Uri inválida");
        //        return;
        //    }

        //    ConfiguredEndpoint endpoint;

        //    try
        //    {
        //        var endpointDescription = await CoreClientUtils.SelectEndpointAsync(config, endpointURL, false, ct);
        //        var endpointConfig = Opc.Ua.EndpointConfiguration.Create(config);
        //        endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfig);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger?.LogError(ex, "Error seleccionando endpoint {Endpoint} para Instalación {Instalacion}", endpointURL, bateria.IdBateria);
        //        await _opcNegocio.CambiarEstado(Skysense_models.Enums.Entidad.Bateria, bateria.IdBateria, 5, "Error de endpoint");
        //        return;
        //    }


        //    // Crear sesión sin certificados
        //    Session session;
        //    try
        //    {
        //        session = await Session.Create(
        //            config,
        //            endpoint,
        //            false,
        //            "ConsoleSession",
        //            60000,
        //            null, null, ct);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger?.LogError(ex, "Error creando sesión OPC UA para {Endpoint} (Instalación {Inst}): {mensajeError}", endpointURL, bateria.IdBateria, ex.Message);
        //        await _opcNegocio.CambiarEstado(Skysense_models.Enums.Entidad.Bateria, bateria.IdBateria, 5, "Error creando sesión");
        //        return;
        //    }

        //    // Crear suscripción
        //    var subscription = new Subscription(session.DefaultSubscription)
        //    {
        //        PublishingInterval = _publishingInterval
        //    };

        //    var tagsMonitoreados = new List<TagMonitoreado>();

        //    // Crear MonitoredItems para cada TAG solo si existe en servidor
        //    foreach (var tag in bateria.TagsBateria!.Where(t => t.Monitorear))
        //    {
        //        var nodeId = new NodeId("Tags." + tag.Etiqueta, 2);

        //        bool exists = false;
        //        try
        //        {
        //            exists = await NodeExists(session, nodeId, ct);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger?.LogWarning(ex, "Error comprobando existencia de nodo {NodeId} en servidor para Instalación {Inst}", nodeId, bateria.IdBateria);
        //            await _opcNegocio.CambiarEstado(Skysense_models.Enums.Entidad.Tag, tag.IdTag, 5, "Nodo <<" + nodeId + ">> no encontrado: " + ex.Message);
        //            exists = false;
        //        }

        //        if (!exists)
        //        {
        //            _logger?.LogWarning("Nodo no encontrado en servidor OPC UA: {NodeId} (Instalación {Inst}) - se omitirá monitoreo.", nodeId, bateria.IdBateria);
        //            await _opcNegocio.CambiarEstado(Skysense_models.Enums.Entidad.Tag, tag.IdTag, 5, "Nodo <<" + nodeId + ">> no encontrado.");
        //            continue;
        //        }

        //        var item = new MonitoredItem(subscription.DefaultItem)
        //        {
        //            StartNodeId = nodeId,
        //            AttributeId = Attributes.Value,
        //            DisplayName = nodeId.ToString(),
        //            SamplingInterval = 1000,
        //            QueueSize = 10,
        //            DiscardOldest = true
        //        };
        //        item.Notification += (s, e) => _ = OnNotification(s, e, bateria.IdBateria, tag);
        //        subscription.AddItem(item);

        //        subscription.PublishStatusChanged += (s, e) => _logger?.LogError("Status changed: {0}, {1}", s, e.Status);

        //        tagsMonitoreados.Add(new TagMonitoreado()
        //        {
        //            IdTag = tag.IdTag,
        //            MonitoredItem = item
        //        });

        //        await _opcNegocio.CambiarEstado(Skysense_models.Enums.Entidad.Tag, tag.IdTag, 2);

        //    }

        //    session.AddSubscription(subscription);
        //    try
        //    {
        //        await subscription.CreateAsync(ct);
        //        _subscriptions.Add(new BateriaSuscripcion() { Suscripcion = subscription, IdBateria = bateria.IdBateria, TagsMonitoreados = tagsMonitoreados, CadenaConexion = bateria.CadenaConexion });
        //        await _opcNegocio.CambiarEstado(Skysense_models.Enums.Entidad.Bateria, bateria.IdBateria, 2);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger?.LogError(ex, "Error creando la suscripción para Instalación {Inst}", bateria.IdBateria);
        //        await _opcNegocio.CambiarEstado(Skysense_models.Enums.Entidad.Bateria, bateria.IdBateria, 5, "Error creando suscripción: " + ex.Message);
        //    }
        //}

        private async Task DetenerMonitoreo<T>(int idEntidad, CancellationToken ct = default)
        {
            //switch (typeof(T))
            //{
            //    case Type t when t == typeof(OPCBateria):
            //        var bateria = bateriasMonitoreando.FirstOrDefault(b => b.IdBateria == idEntidad);
            //        if (bateria != null)
            //        {
            //            await bateria.Suscripcion!.Session.CloseSessionAsync(null, true, ct);
            //            _subscriptions.TryTake(out var subscription);

            //        }
            //        await _opcNegocio.CambiarEstado(Skysense_models.Enums.Entidad.Bateria, idEntidad, 1, "Monitoreo detenido");
            //        break;
            //    case Type t when t == typeof(TagBateria):
            //        var tag = _subscriptions.SelectMany(s => s.TagsMonitoreados).FirstOrDefault(t => t.IdTag == idEntidad);
            //        if (tag != null)
            //        {
            //            tag.MonitoredItem!.Subscription.RemoveItem(tag.MonitoredItem);
            //            _subscriptions.FirstOrDefault(s => s.TagsMonitoreados.Any(t => t.IdTag == idEntidad))!.TagsMonitoreados.Remove(tag);
            //        }
            //        await _opcNegocio.CambiarEstado(Skysense_models.Enums.Entidad.Tag, idEntidad, 1, "Monitoreo detenido");
            //        break;
            //    default:
            //        break;
            //}
        }

        public async Task ActualizaMonitoreo(int idBateria, CancellationToken ct = default)
        {
            //Task<OPCBateria[]> taskBateria = this._opcNegocio.ObtenConfiguracionesOPC(idBateria);
            ////Detenemos el monitoreo completo
            //await Task.WhenAll(this.DetenerMonitoreo<OPCBateria>(idBateria), taskBateria);

            //var bateria = taskBateria.Result;

            //if (bateria != null && bateria.Length > 0 && bateria[0].Monitorear)
            //{
            //    await this.EmpezarMonitoreo(bateria[0], ct);
            //}

        }

        /// <summary>
        /// Comprueba si un nodo existe en el servidor OPC UA leyendo su valor y verificando el StatusCode.
        /// </summary>
        //private async Task<bool> NodeExists(Session session, NodeId nodeId, CancellationToken ct)
        //{
        //    try
        //    {
        //        // Leer atributo Value; si el nodo no existe, el StatusCode será malo o lanzará ServiceResultException.
        //        var dv = await session.ReadValueAsync(nodeId, ct);
        //        return StatusCode.IsGood(dv.StatusCode);
        //    }
        //    catch (ServiceResultException sre)
        //    {
        //        // Nodo no encontrado u otros errores reportados por el servidor.
        //        _logger?.LogDebug(sre, "ServiceResultException al leer nodo {NodeId}", nodeId);
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger?.LogDebug(ex, "Excepción al verificar nodo {NodeId}", nodeId);
        //        return false;
        //    }
        //}

        //private async Task OnNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e, int IdBateria, TagBateria Tag)
        //{
        //    try
        //    {
        //        var values = item.DequeueValues();
        //        var mediciones = values.Select(v =>
        //        {
        //            decimal medicionDecimal = 0m;
        //            try
        //            {
        //                if (v.Value != null)
        //                {
        //                    medicionDecimal = Convert.ToDecimal(v.Value);
        //                }
        //            }
        //            catch
        //            {
        //                // si falla la conversión, deja 0 o registra el error
        //            }

        //            return new TagMedicion
        //            {
        //                IdTag = Tag.IdTag,
        //                Medicion = medicionDecimal,
        //                Timestamp = v.SourceTimestamp
        //            };
        //        }).ToArray();

        //        var datoAEnviar = new OPCBateria
        //        {
        //            IdBateria = IdBateria,
        //            TagsBateria = new[]
        //            {
        //                new TagBateria
        //                {
        //                    IdTag = Tag.IdTag,
        //                    Mediciones = mediciones
        //                }
        //            }
        //        };

        //        await Task.WhenAll(
        //            _opcNegocio.GuardarMediciones(mediciones, _subscriptions.FirstOrDefault(s => s.IdBateria == IdBateria)?.CadenaConexion),
        //            _hub.Clients.Group("MonitoreoBateriaID" + IdBateria).SendAsync("MonitoreoBateria", datoAEnviar)
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger?.LogError(ex, "Error en OnNotification para idBateria {Id}: {message}", IdBateria, ex.Message);
        //    }
        //}

        #region Internal Methods
        internal void Loguear(LogLevel logLevel, string mensaje, params object?[] args)
        {
            _logger.Log(logLevel, mensaje, args);
        }
        internal async Task ActualizaEstado(Entidad entidad, int idEntidad, byte nuevoEstado, string mensajeEstado)
        {
            await this._opcNegocio.CambiarEstado(entidad, idEntidad, nuevoEstado, mensajeEstado);
        }
        internal Task GuardaDatos(OPCBateria datoAEnviar, string cadenaConexion)
        {
            return Task.WhenAll(
                this._opcNegocio.GuardarMediciones(datoAEnviar.TagsBateria?.FirstOrDefault()?.Mediciones ?? [], cadenaConexion),
                this._hub.Clients.Group("MonitoreoBateriaID" + datoAEnviar.IdBateria).SendAsync("MonitoreoBateria", datoAEnviar)
            );
        }
        #endregion

        private class OPC_Bateria(int idBateria, OPC_BateriaConfiguracion configuracion, OPCDataPolling ctx)
        {
            private const int _publishingInterval = 1000;
            private const int _refreshInterval = 1000;
            private int _idBateria = idBateria;
            private OPCDataPolling _context = ctx;
            private EstatusMonitoreoBDD _estatus;

            private List<OPC_Variable> _variablesL = new List<OPC_Variable>();

            private Opc.Ua.Client.Session? _session;

            private Opc.Ua.Client.Subscription? _suscripcion;

            private bool _monitorear;

            private OPC_BateriaConfiguracion config { get; set; } = configuracion;

            public int IdBateria { get { return _idBateria; } }
            public string CadenaConexion { get { return config.CadenaConexion; } }

            public void AgregaVariable(int idVariable, OPC_VariableConfiguracion configuracion)
            {
                var variable = new OPC_Variable(this._idBateria, idVariable, configuracion, _context, this);
                _variablesL.Add(variable);
            }

            public async Task EmpezarAMonitorear(CancellationToken ct)
            {
                if (_monitorear)
                    return;

                //1. Se crea la configuración de los certificados
                var certificadosConfig = await ConfigurarCertificados(ct);

                //2. Intentamos crear el endpoint
                if (!System.Uri.TryCreate(this.config.UrlEMS, UriKind.Absolute, out Uri? result))
                {
                    _context.Loguear(LogLevel.Warning, "URI inválida para la batería {IdInstalacion}: {Url}", this._idBateria, this.config.UrlEMS);
                    await _context.ActualizaEstado(Entidad.Bateria, _idBateria, 5, "Uri inválida");
                    return;
                }

                ConfiguredEndpoint endpoint;

                try
                {
                    var endpointDescription = await CoreClientUtils.SelectEndpointAsync(certificadosConfig, this.config.UrlEMS, false, ct);
                    var endpointConfig = Opc.Ua.EndpointConfiguration.Create(certificadosConfig);
                    endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfig);
                }
                catch (Exception ex)
                {
                    _context.Loguear(LogLevel.Error, "Error seleccionando endpoint {Endpoint} para Instalación {Instalacion}: {ex}", this.config.UrlEMS, _idBateria, ex.Message);
                    await _context.ActualizaEstado(Entidad.Bateria, _idBateria, 5, "Error de endpoint");
                    return;
                }


                //3. Crear sesión sin certificados
                try
                {
                    this._session = await Session.Create(
                        certificadosConfig,
                        endpoint,
                        false,
                        "ConsoleSession",
                        60000,
                        null, null, ct);
                }
                catch (Exception ex)
                {
                    _context.Loguear(LogLevel.Error, "Error creando sesión OPC UA para {Endpoint} (Instalación {Inst}): {mensajeError}", this.config.UrlEMS, _idBateria, ex.Message);
                    await _context.ActualizaEstado(Skysense_models.Enums.Entidad.Bateria, _idBateria, 5, "Error creando sesión");
                    return;
                }

                //4. Crear suscripción
                this._suscripcion = new Subscription(_session.DefaultSubscription)
                {
                    PublishingInterval = _publishingInterval
                };
                _session.AddSubscription(_suscripcion);

                //5. Creacion de Monitored Items
                await Task.WhenAll(_variablesL.Select(v=> v.UnirseALaSuscripcion(_suscripcion, ct)));

                //6.Se crea la suscripción en el servidor junto con todos los Monitored Items
                try
                {
                    await _suscripcion.CreateAsync(ct);
                    await _context.ActualizaEstado(Skysense_models.Enums.Entidad.Bateria, _idBateria, 2, "Monitoreando");

                }
                catch (Exception ex)
                {
                    _context.Loguear(LogLevel.Error, "Error creando la suscripción para Instalación {Inst}: {err}", _idBateria, ex.Message);
                    await _context.ActualizaEstado(Skysense_models.Enums.Entidad.Bateria, _idBateria, 5, "Error creando suscripción: " + ex.Message);
                }
            }

            private async Task<ApplicationConfiguration> ConfigurarCertificados(CancellationToken ct)
            {
                var config = new ApplicationConfiguration()
                {
                    ApplicationName = "OpcUaConsoleSubscriber",
                    ApplicationType = ApplicationType.Client,
                    SecurityConfiguration = new SecurityConfiguration
                    {
                        ApplicationCertificate = new CertificateIdentifier(),
                        AutoAcceptUntrustedCertificates = true,
                        AddAppCertToTrustedStore = false,
                        TrustedIssuerCertificates = new CertificateTrustList() { StoreType = "Directory", StorePath = "%CommonApplicationData%\\OPC Foundation\\pki\\issuer" },
                        TrustedPeerCertificates = new CertificateTrustList() { StoreType = "Directory", StorePath = "%CommonApplicationData%\\OPC Foundation\\pki\\trusted" },
                    },
                    TransportConfigurations = new TransportConfigurationCollection(),
                    TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                    ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
                };
                await config.ValidateAsync(ApplicationType.Client, ct);

                return config;
            }
        }

        private class OPC_Variable(int idBateria, int idVariable, OPC_VariableConfiguracion config, OPCDataPolling ctx, OPC_Bateria bateriaRef)
        {
            private const int _samplingInterval = 1000;

            private int _idBateria = idBateria;
            private int _idVariable = idVariable;
            private EstatusMonitoreoBDD _estatus;
            //private Opc.Ua.Client.Subscription _suscripcion;
            private OPC_VariableConfiguracion _config = config;
            private OPCDataPolling _parentRef = ctx;
            private OPC_Bateria _bateriaRef = bateriaRef;

            public Opc.Ua.Client.MonitoredItem Item { get; set; }

            internal async Task UnirseALaSuscripcion(Subscription suscripcion, CancellationToken ct)
            {
                var nodeId = new NodeId("Tags." + this._config.Tag, 2);
                bool exists = false;
                try
                {
                    exists = await NodeExists(suscripcion.Session, nodeId, ct);
                }
                catch (Exception ex)
                {
                    _parentRef.Loguear(LogLevel.Warning, "Error comprobando existencia de nodo {NodeId} en servidor para Instalación {Inst}", nodeId, _idBateria);
                    await _parentRef.ActualizaEstado(Entidad.Tag, this._idVariable, 5, "Nodo <<" + nodeId + ">> no encontrado: " + ex.Message);
                    exists = false;
                }

                if (!exists)
                {
                    _parentRef.Loguear(LogLevel.Warning, "Nodo no encontrado en servidor OPC UA: {NodeId} (Instalación {Inst}) - se omitirá monitoreo.", nodeId, _idBateria);
                    await _parentRef.ActualizaEstado(Skysense_models.Enums.Entidad.Tag, this._idVariable, 5, "Nodo <<" + nodeId + ">> no encontrado.");
                    return;
                }

                this.Item = new MonitoredItem(suscripcion.DefaultItem)
                {
                    StartNodeId = nodeId,
                    AttributeId = Attributes.Value,
                    DisplayName = nodeId.ToString(),
                    SamplingInterval = _samplingInterval,
                    QueueSize = 10,
                    DiscardOldest = true,
                    
                };

                Item.Notification += (s, e) => _ = OnNotification(s, e);
                suscripcion.AddItem(Item);

                //subscription.PublishStatusChanged += (s, e) => _logger?.LogError("Status changed: {0}, {1}", s, e.Status);

                await _parentRef.ActualizaEstado(Entidad.Tag, this._idVariable, 2, "Monitoreando");
            }

            private async Task<bool> NodeExists(Opc.Ua.Client.ISession session, NodeId nodeId, CancellationToken ct)
            {
                try
                {
                    // Leer atributo Value; si el nodo no existe, el StatusCode será malo o lanzará ServiceResultException.
                    var dv = await session.ReadValueAsync(nodeId, ct);
                    return StatusCode.IsGood(dv.StatusCode);
                }
                catch (ServiceResultException sre)
                {
                    // Nodo no encontrado u otros errores reportados por el servidor.
                    _parentRef.Loguear(LogLevel.Debug, "ServiceResultException al leer nodo {NodeId}: {sre}", nodeId, sre.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    _parentRef.Loguear(LogLevel.Debug, "Excepción al verificar nodo {NodeId}: {ex}", nodeId, ex.Message);
                    return false;
                }
            }

            private async Task OnNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
            {
                try
                {
                    var values = item.DequeueValues();
                    var mediciones = values.Select(v =>
                    {
                        decimal medicionDecimal = 0m;
                        try
                        {
                            if (v.Value != null)
                            {
                                medicionDecimal = Convert.ToDecimal(v.Value);
                            }
                        }
                        catch
                        {
                            // si falla la conversión, deja 0 o registra el error
                        }

                        return new TagMedicion
                        {
                            IdTag = this._idVariable,
                            Medicion = medicionDecimal,
                            Timestamp = v.SourceTimestamp
                        };
                    }).ToArray();

                    var datoAEnviar = new OPCBateria
                    {
                        IdInstalacion = _idBateria,
                        IdBateria = _idBateria,
                        TagsBateria = new[]
                        {
                        new TagBateria
                        {
                            IdTag = _idVariable,
                            Mediciones = mediciones
                        }
                    }
                    };

                    await _parentRef.GuardaDatos(datoAEnviar, _bateriaRef.CadenaConexion);
                }
                catch (Exception ex)
                {
                    _parentRef.Loguear(LogLevel.Error, "Error en OnNotification para idBateria {Id}: {message}", _idBateria, ex.Message);
                }
            }
        }

        private class OPC_BateriaConfiguracion
        {
            public bool Monitorear { get; set; }
            public string UrlEMS { get; set; }
            public string CadenaConexion { get; set; }

        }

        private class OPC_VariableConfiguracion
        {
            public bool Monitorear { get; set; }
            public string Tag { get; set; }
            public string NombreAMostrar { get; set; }
            public int? IdAgrupacionGrafica { get; set; }
            public bool EsGraficable { get; set; }
            public bool EsSoloLectura { get; set; }
            public string Unidad { get; set; }
        }

        private enum EstatusMonitoreoBDD
        {
            Detenido = 0x01,
            Monitoreando = 0x02,
            ErrorMonitoreando = 0x03,
            MonitoreandoConErrores = 0x04,
            ErrorConfiguracion = 0x05
        }
    

    }

    class BateriaSuscripcion
    {
        public int IdBateria { get; set; }
        public Subscription? Suscripcion { get; set; }
        public string? CadenaConexion { get; set; }

        public List<TagMonitoreado> TagsMonitoreados { get; set; } = [];
    }

    class TagMonitoreado
    {
        public int IdTag { get; set; }
        public MonitoredItem? MonitoredItem { get; set; }
    }


}
