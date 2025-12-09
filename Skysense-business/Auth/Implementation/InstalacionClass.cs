using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Skysense_business.APIPlataformas;
using Skysense_business.Auth.Interfaces;
using Skysense_business.Comun;
using Skysense_Data.Auth.Implementation;
using Skysense_Data.Auth.Interfaces;
using Skysense_models;
using Skysense_models.DB;
using Skysense_models.DB.FunctionReturns;
using Skysense_models.Errors;
using Skysense_models.Otros;
using Skysense_models.Peticiones;
using Skysense_models.Respuestas;
using System.Collections.Generic;
using System.Globalization;

namespace Skysense_business.Auth.Implementation
{
    public class InstalacionClass : IInstalaciones
    {
        private readonly Skysense_Data.Auth.Interfaces.IInsatalaciones _instalaciones;
        private readonly Skysense_Data.Auth.Interfaces.ICliente _cliente;
        private readonly Skysense_Data.Auth.Interfaces.IInterfazData _interfaz;
        private readonly Skysense_Data.Auth.Interfaces.IAPIWorker _apiworker;
        private readonly IConfiguration _config;

        public InstalacionClass(Skysense_Data.Auth.Interfaces.IInsatalaciones instalaciones, Skysense_Data.Auth.Interfaces.IAPIWorker apiworker, IConfiguration config, ICliente cliente, IInterfazData interfaz)
        {
            this._instalaciones = instalaciones;
            this._apiworker = apiworker;
            this._config = config;
            this._cliente = cliente;
            this._interfaz = interfaz;
        }

        public Paginacion<CInstalacione> mObtenInstalaciones(int? idGrupo, string? busqueda, int paginaPaginacion, int registrosPorPagina)
        {
            return _instalaciones.mObtenInstalacionesPaginadas(idGrupo, busqueda, paginaPaginacion, registrosPorPagina);
        }

        public CInstalacione? mActualizaInstalacion(CInstalacione cInstalacion)
        {
            return _instalaciones.mActualizaInstalacion(cInstalacion);
        }

        public bool mEliminaInstalacion(CInstalacione cInstalacion)
        {
            return _instalaciones.mEliminaInstalacion(cInstalacion);
        }

        public CInstalacione[] mObtenInstalacion(int idCliente, int? idInstalacion = null)
        {
            return _instalaciones.mObtenInstalaciones(idCliente, idInstalacion);
        }

        public CInstalacione mNuevaInstalacion(CInstalacione cInstalacion)
        {
            return _instalaciones.mNuevaInstalacion(cInstalacion);
        }

        public async Task<TablaGeneracion> mObtenGeneracion(int idCliente, int idInstalacion, int iAnion)
        {
            var insta = this.mObtenInstalacion(idCliente, idInstalacion).FirstOrDefault();
            var valoresG = this._instalaciones.mObtenValoresGarantizados(idInstalacion, iAnion).ToList();

            for (int i = 0; i < 12; i++)
            {
                if (!valoresG.Any(v => v.Mes == i + 1))
                    valoresG.Add(new CInstalacionApigeneracionMensual()
                    {
                        IdInstalacion = idInstalacion,
                        Anno = (short)iAnion,
                        Mes = (byte)(i + 1),
                        DaemonYaEjecutado = false,
                        GeneracionGarantizada = null
                    });
            }

            valoresG = valoresG.OrderBy(v => v.Mes).ToList();

            var tabla = new TablaGeneracion()
            {
                idInstalacion = idInstalacion,
                dtFechaInicioOperaciones = insta?.InicioOperaciones ?? DateOnly.MaxValue,
                fPorcentajeDesgaste = insta?.PorcentajeDesgastePaneles ?? 0,
                arrValoresGarantizados = valoresG.ToArray(),
                arrValoresReales = new decimal?[12]
            };

            if (tabla.arrValoresGarantizados.Any(v => !v.DaemonYaEjecutado))
            {
                ApiPlataforma apiPlataforma;
                switch (insta.IdPlataforma)
                {
                    case 5:
                        apiPlataforma = new ApiPlataformaSolis();
                        break;
                    case 3:
                        apiPlataforma = new ApiPlataformaFusionSolar();
                        break;
                    case 6:
                        apiPlataforma = new ApiPlataformaSungrow();
                        break;
                    default:
                        throw new NotImplementedException("No se cuenta con la implementación de esta plataforma.");
                }

                var task1 = apiPlataforma.mObtenGeneracionAnualInversor(iAnion, insta.IdInstalacionApi ?? "");
                var task2 = _instalaciones.mObtenDataInversoresAjustada(idInstalacion, iAnion);

                await Task.WhenAll(task1, task2);


                tabla.arrValoresReales = task1.Result;
                var results2 = task2.Result;

                if (tabla.arrValoresReales == null)
                {
                    return null;
                }

                for (int i = 0; i < tabla.arrValoresReales.Count(); i++)
                {
                    tabla.arrValoresReales[i] += results2.Where(r => r.FechaRegistro.Value.Month == i + 1).Sum(r => r.ValorNuevo - r.ValorOriginal);
                }
            }

            var valoresRealesIntroducidos = this._instalaciones.mObtenValoresRealesDB(idInstalacion, iAnion);//.OrderBy(r => r.Mes).Select(r => r.GeneracionMes).ToArray();

            foreach (var mes in valoresRealesIntroducidos.Where(vr=> tabla.arrValoresGarantizados[vr.Mes!.Value-1].DaemonYaEjecutado == true))
            {
                tabla.arrValoresReales[mes.Mes!.Value - 1] = mes.GeneracionMes;
            }




            //for (int i = 0; i < tabla.arrValoresGarantizados.Length; i++)
            //{
            //    if (iAnion == tabla.dtFechaInicioOperaciones.Year && i + 1 < tabla.dtFechaInicioOperaciones.Month)
            //        tabla.arrValoresGarantizados[i].GeneracionGarantizada = null;
            //    else
            //    {
            //        tabla.arrValoresGarantizados[i] =
            //            tabla.arrValoresGarantizados[i] * (100 -
            //            tabla.fPorcentajeDesgaste *
            //            (iAnion - tabla.dtFechaInicioOperaciones.Year -
            //            ((i + 1) < tabla.dtFechaInicioOperaciones.Month ? 1 : 0))) / 100;
            //    }

            //}

            //tabla.arrValoresReales = new decimal?[12];

            return tabla;


        }

        public async Task<TablaGeneracionDiaria> mObtenGeneracionDiaria(int idCliente, int idInstalacion, int iAnio, int iMes)
        {
            var insta = this.mObtenInstalacion(idCliente, idInstalacion).FirstOrDefault();
            var generacionMensual = this._instalaciones.mObtenValoresGarantizados(idInstalacion, iAnio);
            InversorDataDiaria[] inversoresDiarios;

            //Si ya hay generación almacenada
            if ((generacionMensual.FirstOrDefault(g => g.Mes == iMes)?.DaemonYaEjecutado ?? false) == true)
            {
                inversoresDiarios = this._instalaciones.mObtenGeneracionDiariaBD(idInstalacion, iAnio, iMes);
            }
            else
            {
                ApiPlataforma apiPlataforma;
                switch (insta.IdPlataforma)
                {
                    case 5:
                        apiPlataforma = new ApiPlataformaSolis();
                        break;
                    case 3:
                        apiPlataforma = new ApiPlataformaFusionSolar();
                        break;
                    case 6:
                        apiPlataforma = new ApiPlataformaSungrow();
                        break;
                    default:
                        throw new NotImplementedException("No se cuenta con la implementación de esta plataforma.");
                }

                var task1 = apiPlataforma.mObtenGeneracionDiariaInversores(insta.IdInstalacionApi, new DateTime(iAnio, iMes, 5));
                var task2 = _instalaciones.mObtenDataInversoresAjustada(idInstalacion, iAnio, iMes);

                await Task.WhenAll(task1, task2);

                inversoresDiarios = task1.Result;

                var task3 = this.mInsertaGeneracionMensualDiariaAPP(idInstalacion, new DateOnly(iAnio, iMes, 15), inversoresDiarios);

                var datosAjustados = task2.Result.GroupBy(g => g.IdentificadorInversor);

                foreach (var dato in datosAjustados)
                {
                    var inversor = inversoresDiarios.FirstOrDefault(i => i.identificador == dato.Key);
                    if (inversor == null)
                    {
                        inversor = new InversorDataDiaria()
                        {
                            identificador = dato.Key,
                            numeroSerie = "",
                            encabezado = dato.Key,
                            inversorData = dato.Select(d => new InversorData()
                            {
                                fecha = d.FechaRegistro.Value.ToDateTime(TimeOnly.MinValue),
                                generacion = d.ValorNuevo ?? 0
                            }).ToArray()
                        };
                        inversoresDiarios?.Append(inversor);
                    }
                    else
                    {
                        var lista = inversor.inversorData.ToList();
                        //Se agregan valores que puede que hayan sido modificados pero no estaban en la consulta
                        lista.AddRange(dato.Where(d => !inversor.inversorData.Any(i => i.fecha == d.FechaRegistro.Value.ToDateTime(TimeOnly.MinValue)))
                                    .Select(d => new InversorData()
                                    {
                                        fecha = d.FechaRegistro.Value.ToDateTime(TimeOnly.MinValue),
                                        generacion = d.ValorNuevo ?? 0,
                                        esEditado = true
                                    }));

                        inversor.inversorData = lista.ToArray();

                        foreach (var invData in inversor.inversorData)
                        {
                            var coincidenciaFecha = dato.FirstOrDefault(d => d.FechaRegistro.Value.ToDateTime(TimeOnly.MinValue) == invData.fecha);
                            if (coincidenciaFecha != null)
                            {
                                invData.generacion = coincidenciaFecha.ValorNuevo ?? 0;
                                invData.esEditado = true;
                            }
                        }
                    }
                }

                await Task.WhenAll(task3);
            }

            var tabla = new TablaGeneracionDiaria()
            {
                inversoresEncabezados = inversoresDiarios.Select(i => i.encabezado).ToArray(),
                inversoresIdentificadores = inversoresDiarios.Select(i => i.identificador).ToArray()
            };

            var diasMes = new DateTime(iAnio, iMes, 1).AddMonths(1).AddDays(-1).Day;
            var renglonesTabla = new List<RenglonGeneracionDiaria>();
            for (int i = 1; i <= diasMes; i++)
            {
                var fechaABuscar = new DateTime(iAnio, iMes, i);
                var diaInversores = inversoresDiarios.Select(inv => inv.inversorData.FirstOrDefault(inv1 => MetodosComunes.FechasIguales(inv1.fecha, fechaABuscar)));

                renglonesTabla.Add(new RenglonGeneracionDiaria()
                {
                    iDia = i,
                    arregloEditados = diaInversores.Select(d => d?.esEditado ?? false).ToArray(),
                    //datosInversores = inversoresDiarios.Select(inv => inv.inversorData.FirstOrDefault(inv1 => MetodosComunes.FechasIguales(inv1.fecha, fechaABuscar))?.generacion).ToArray()
                    datosInversores = diaInversores.Select(d => d?.generacion).ToArray()
                });

                //Se calcula la desviacion maxima como la sumatoria de los absolutos de la diferencia entre el valor y el promedio. Al final dividido entre numero de registros
                renglonesTabla[i - 1].generacionTotal = renglonesTabla[i - 1].datosInversores.Sum(di => di ?? 0);
                var cuantos = Math.Max(1, renglonesTabla[i - 1].datosInversores.Count(di => di != null));
                var promedio = renglonesTabla[i - 1].generacionTotal / cuantos;
                renglonesTabla[i - 1].desviacionMaxima = renglonesTabla[i - 1].datosInversores.Where(di => di != null).Sum(di => Math.Abs(di!.Value - promedio)) / cuantos;

            }

            tabla.datos = renglonesTabla.ToArray();

            return tabla;
        }

        private async Task mInsertaGeneracionMensualDiariaAPP(int idInstalacion, DateOnly dateOnly, InversorDataDiaria[] inversoresDiarios)
        {

            var instalacionWk = await this._apiworker.ObtenerInstalacion(idInstalacion);

            //Si hay algun inversor que no este en la tabla de inversores API, se da de alta
            var invNuevos = inversoresDiarios
                .GroupBy(g => g.identificador)
                .Where(g => !instalacionWk.InversoresAPI.Any(ia => ia.IdApi == g.Key))
                .Select(g => new CInversorApiencabezado
                {
                    IdApi = g.Key,
                    NumeroSerie = g.First().numeroSerie,
                    FechaUltimaActualizacion = DateTime.Now,
                    IdInstalacion = idInstalacion,
                    IdInversor = instalacionWk.InversoresAPP.FirstOrDefault(i => i.NumeroSerie == g.First().numeroSerie)?.IdInversor ?? 0
                });

            if (invNuevos != null && invNuevos.Count() > 0)
            {
                var nuevosInvs = await this._apiworker.DarDeAltaInversoresAPI(invNuevos.ToList());
                instalacionWk.InversoresAPI = instalacionWk.InversoresAPI.Concat(nuevosInvs).ToList();
            }

            if (inversoresDiarios != null && inversoresDiarios.Count() > 0 && instalacionWk != null)
            {
                inversoresDiarios.GroupBy(g => g.identificador);

                var listaGeneracion = inversoresDiarios.Select(invD => invD.inversorData.Select(invG => new CInversorApigeneracion()
                {
                    IdInversorApi = instalacionWk.InversoresAPI.FirstOrDefault(i => i.IdApi == invD.identificador)?.IdInversorApi ?? 0,
                    FechaUltimaActualizacion = DateTime.Now,
                    FechaValor = invG.fecha,
                    Valor = invG.generacion,
                    IdConsecutivo = 0,
                    ValorManual = false
                })).SelectMany(x => x);

                //await _instalaciones.mActualizaInversoresAPI(instalacionWk.IdInstalacion, instalacionWk.InversoresAPI);

                await this._apiworker.ActualizaGeneracionDiaria(listaGeneracion);

                await this._apiworker.mActualizaGeneracionMensualParametros(instalacionWk.IdInstalacion, dateOnly.Month, dateOnly.Year);
            }
        }

        public async Task<MethodResponse<int>> mInsertaNuevosPaneles(int idInstalacion, IEnumerable<CPanele> paneles)
        {
            if (paneles.Count() == 0)
                return new MethodResponse<int>(true, "Sin datos a insertar.");

            var registros = await this._instalaciones.mInsertaNuevosPaneles(idInstalacion, paneles);

            return new MethodResponse<int>(true, $"Se han insertado {registros} paneles.");
        }

        public CPanele[] mObtenPaneles(int idInstalacion)
        {
            return this._instalaciones.mObtenPaneles(idInstalacion);
        }

        public Paginacion<Documento> mObtenDocumentos(int idInstalacion, int paginaPaginacion, int registrosPorPagina, int tipoDocumento)
        {
            return this._instalaciones.mObtenDocumentos(idInstalacion, paginaPaginacion, registrosPorPagina, tipoDocumento);
        }

        public async Task<Documento> mInsertaModificaDocumento(Documento dcto, IFormFile archivo, DateOnly? iAnno)
        {
            string archivoFullRuta = "";
            string extension = "";
            string archivoFullPapelera = "";

            //SI es modificación
            if (dcto.idDocumento > 0)
            {
                CDocumento dctoBDD = this._instalaciones.fObtenDocumento(dcto.idInstalacion, dcto.idDocumento);

                archivoFullRuta = Path.Combine(_config["Rutas_Documentos:Instalaciones"]!, dctoBDD.RutaRelativa);
                extension = Path.GetExtension(dctoBDD.RutaRelativa);
                archivoFullPapelera = Path.Combine(_config["Rutas_Documentos:Papelera"]!, $"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}{extension}");

                if (File.Exists(archivoFullRuta))
                {
                    File.Move(archivoFullRuta, archivoFullPapelera, true);
                }
            }

            //Se inserta el nuevo documento
            var nuevoDocumento = await mInsertaDocumento(dcto, archivo);

            try
            {
                //Se inserta en la base de datos
                var idDoc = await this._instalaciones.mInsertaModificaDocumento(nuevoDocumento);

                if (iAnno.HasValue)
                {
                    if (dcto.tipoDocumento == 1) //Reportes
                    {
                        //Se inserta el recibo
                        await this._instalaciones.mInsertaReporte(new CReporte()
                        {
                            IdInstalacion = dcto.idInstalacion,
                            MesReporte = iAnno.Value,
                            IdDocumento = idDoc
                        });
                    }
                    else if (dcto.tipoDocumento == 2) //Recibo
                    {
                        //Se inserta el reporte
                        await this._instalaciones.mInsertaRecibo(new CRecibo()
                        {
                            IdInstalacion = dcto.idInstalacion,
                            MesRecibo = iAnno.Value,
                            IdDocumento = idDoc
                        });
                    }
                }

            }
            catch (Exception)
            {
                if (dcto.idDocumento > 0)
                {
                    File.Move(archivoFullPapelera, archivoFullRuta, true);
                }
                throw;
            }

            //Si es modificación, se elimina el archivo anterior
            if (dcto.idDocumento > 0 && File.Exists(archivoFullPapelera))
            {
                File.Delete(archivoFullPapelera);
            }

            return new Documento()
            {
                idDocumento = nuevoDocumento.IdDocumento,
                idInstalacion = nuevoDocumento.IdInstalacion,
                tipoDocumento = nuevoDocumento.TipoDocumento,
                nombreDocumento = nuevoDocumento.NombreDocumento,
                tipoArchivo = nuevoDocumento.TipoArchivo,
                peso = nuevoDocumento.Peso,
                fechaModificacion = nuevoDocumento.FechaModificacion
            };
        }

        private async Task<CDocumento> mInsertaDocumento(Documento dcto, IFormFile archivo)
        {
            var extension = Path.GetExtension(archivo.FileName);
            var nombreBase = Path.GetFileNameWithoutExtension(archivo.FileName);

            var directorioDocumentos = _config["Rutas_Documentos:Instalaciones"]!;
            var rutaRelativa = Path.Combine(dcto.idInstalacion.ToString(), dcto.tipoDocumento.ToString());

            var nombreUnico = GeneraNombreArchivoUnico(Path.Combine(directorioDocumentos, rutaRelativa), nombreBase, extension);

            var rutaDestino = Path.Combine(directorioDocumentos, rutaRelativa);
            var rutaCompleta = Path.Combine(rutaDestino, nombreUnico);

            Directory.CreateDirectory(rutaDestino);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return new CDocumento()
            {
                IdInstalacion = dcto.idInstalacion,
                TipoDocumento = dcto.tipoDocumento,
                NombreDocumento = Path.GetFileNameWithoutExtension(nombreUnico),
                TipoArchivo = extension,
                Peso = (int)archivo.Length,
                FechaModificacion = DateTime.Now,
                RutaRelativa = Path.Combine(rutaRelativa, nombreUnico)
            };
        }

        private string GeneraNombreArchivoUnico(string directorio, string nombreBase, string extension)
        {
            var nombreArchivo = $"{nombreBase}{extension}";
            var rutaCompleta = Path.Combine(directorio, nombreArchivo);
            int contador = 1;
            while (File.Exists(rutaCompleta))
            {
                nombreArchivo = $"{nombreBase}_{contador}{extension}";
                rutaCompleta = Path.Combine(directorio, nombreArchivo);
                contador++;
            }
            return nombreArchivo;
        }

        public async Task mEliminaDocumento(int idInstalacion, int idDocumento)
        {
            CDocumento dctoBDD = this._instalaciones.fObtenDocumento(idInstalacion, idDocumento);
            string archivoFullRuta = Path.Combine(_config["Rutas_Documentos:Instalaciones"]!, dctoBDD.RutaRelativa);
            if (File.Exists(archivoFullRuta))
            {
                File.Delete(archivoFullRuta);
            }
            await this._instalaciones.mEliminaDocumento(idInstalacion, idDocumento);
            return;
        }

        public CDocumento mObtenDocumento(int idInstalacion, int idDocumento)
        {
            return this._instalaciones.fObtenDocumento(idInstalacion, idDocumento);
        }

        CGrupo[] IInstalaciones.mObtenGrupos()
        {
            return this._instalaciones.mObtenGrupos();
        }

        public void mModificaPanel(CPanele panele)
        {
            this._instalaciones.mModificaPanel(panele);
            return;
        }

        public void EliminarPanel(int idPanel)
        {
            this._instalaciones.EliminarPanel(idPanel);
            return;

        }

        public CReporte[] mObtenReportesPorInstalacion(int idInstalacion, int iAnnio)
        {
            var rep = this._instalaciones.mObtenReportesPorInstalacion(idInstalacion, iAnnio).ToList();

            for (int i = 0; i < 12; i++)
            {
                var rr = rep.Find(r => r.MesReporte.Month == i + 1 && r.MesReporte.Year == iAnnio);
                if (rr == null)
                {
                    rr = new CReporte()
                    {
                        IdInstalacion = idInstalacion,
                        MesReporte = new DateOnly(iAnnio, i + 1, 1),
                        IdDocumento = null,
                        IdDocumentoNavigation = new CDocumento()
                        {
                            IdInstalacion = idInstalacion,
                            TipoDocumento = 2, // Asumiendo que 2 es el tipo de reporte
                            NombreDocumento = "",
                            TipoArchivo = "", // Asumiendo que el tipo de archivo es PDF
                            Peso = 0, // Peso inicial
                            FechaModificacion = DateTime.Now
                        }
                    };

                    rep.Add(rr);
                }

                rr.IdDocumentoNavigation.NombreDocumento = $"Reporte de {rr.MesReporte:MMMM yyyy}";
            }

            return rep.OrderBy(r => r.MesReporte).ToArray();
        }

        public CRecibo[] mObtenRecibosPorInstalacion(int idInstalacion, int iAnnio)
        {
            var rep = this._instalaciones.mObtenRecibosPorInstalacion(idInstalacion, iAnnio).ToList();

            for (int i = 0; i < 12; i++)
            {
                var rr = rep.Find(r => r.MesRecibo.Month == i + 1 && r.MesRecibo.Year == iAnnio);
                if (rr == null)
                {
                    rr = new CRecibo()
                    {
                        IdInstalacion = idInstalacion,
                        MesRecibo = new DateOnly(iAnnio, i + 1, 1),
                        IdDocumento = null,
                        IdDocumentoNavigation = new CDocumento()
                        {
                            IdInstalacion = idInstalacion,
                            TipoDocumento = 2, // Asumiendo que 2 es el tipo de reporte
                            NombreDocumento = "",
                            TipoArchivo = "", // Asumiendo que el tipo de archivo es PDF
                            Peso = 0, // Peso inicial
                            FechaModificacion = DateTime.Now
                        }
                    };

                    rep.Add(rr);
                }

                rr.IdDocumentoNavigation.NombreDocumento = $"Recibo de {rr.MesRecibo:MMMM yyyy}";
            }

            return rep.OrderBy(r => r.MesRecibo).ToArray();
        }

        public void mInsertaModificaReporte(CReporte reporte)
        {
            this._instalaciones.mInsertaModificaReporte(reporte);
        }
        public void mInsertaModificaRecibo(CRecibo recibo)
        {
            this._instalaciones.mInsertaModificaRecibo(recibo);
        }

        public CDocumento[] mObtenDocumentosTipo(int idInstalacion, int[] tipos)
        {
            var documentos = this._instalaciones.mObtenDocumentosTipo(idInstalacion, tipos);
            foreach (var doc in documentos)
            {
                doc.RutaRelativa = "";
            }
            return documentos;
        }

        public Paginacion<PanelSimple> mObtenPanelesPaginados(int pagina, int registrosPorPagina, string? buscador)
        {
            return this._instalaciones.mObtenPanelesPaginados(pagina, registrosPorPagina, buscador);
        }

        public OpcionesCatalogo[] mModificaAgregaCatalogo(int idCatalogoMaestro, int idOpcionCatalogo, string opcion)
        {
            return this._instalaciones.mModificaAgregaCatalogo(idCatalogoMaestro, idOpcionCatalogo, opcion);
        }

        public bool mCambiaGrupo(int idInstalacion, int idGrupoN)
        {
            return this._instalaciones.mCambiaGrupo(idInstalacion, idGrupoN);
        }

        public CInstalacione mObtenInstalacionCompleta(int idInstalacion)
        {
            return this._instalaciones.mObtenInstalacionCompleta(idInstalacion);
        }
        public void mActualizaValoresGarantizados(int idInstalacion, int iAnnio, CInstalacionApigeneracionMensual[] valoresGarantizados)
        {

            if (valoresGarantizados.Any(v => v.Mes > 12 || v.Mes < 1))
            {
                throw new ErrorAlCliente("Los meses deben de estar entre 1 y 12.");
            }
            if (valoresGarantizados.GroupBy(v => v.Mes).Any(g => g.Count() > 1))
            {
                throw new ErrorAlCliente("No se pueden repetir meses.");
            }

            this._instalaciones.mActualizaValoresGarantizados(idInstalacion, iAnnio, valoresGarantizados);
            return;
        }

        public KPIInstalacionesResult[] mObtenKpiAnual(int anno, bool annoGarantia, int? mes)
        {
            if (mes.HasValue && (mes < 1 || mes > 12))
            {
                throw new ErrorAlCliente("El mes debe de estar entre 1 y 12.");
            }
            if (mes.HasValue && annoGarantia)
            {
                throw new ErrorAlCliente("No se puede filtrar por mes cuando se selecciona año de garantía.");
            }
            if (anno < 2000 || anno > DateTime.Now.Year + 10)
            {
                throw new ErrorAlCliente("El año debe de estar entre 2000 y el año actual.");
            }

            return this._instalaciones.mObtenKpiAnual(anno, annoGarantia, mes);
        }

        public async Task mSubeReporteAutomatico(int idInstalacion, IFormFile archivo, DateOnly dateOnly, decimal panelGeneracion, decimal ahorroAcumulado, decimal ahorroAmbiental, decimal consumoCFE, bool bajaTension2)
        {
            var instalacion = this.mObtenInstalacionCompleta(idInstalacion);
            var cliente = instalacion.IdClienteNavigation;
            var dcto = new Documento()
            {
                idInstalacion = idInstalacion,
                tipoDocumento = 1, //Reporte
                nombreDocumento = $"Reporte de {dateOnly:MMMM yyyy}"
            };

            var nuevoDcto = this.mInsertaDocumento(dcto, archivo).Result;
            var idDoc = await this._instalaciones.mInsertaModificaDocumento(nuevoDcto);
            var idRep = await this._instalaciones.mInsertaReporte(new CReporte()
            {
                IdInstalacion = idInstalacion,
                MesReporte = dateOnly,
                IdDocumento = idDoc
            });
            this._instalaciones.mInsertaModificaReporte(new CReporte
            {
                IdInstalacion = idInstalacion,
                IdReporte = idRep,
                AhorroAcumulado = ahorroAcumulado,
                AhorroAmbiental = ahorroAmbiental,
                ConsumoCFE = consumoCFE,
                IdDocumento = idDoc,
                MesReporte = dateOnly,
                PanelesGeneracion = panelGeneracion,
            });
            return;
        }

        public Task<CReporteAutomaticoConfig> mObtenConfiguracionReportesAutomaticos(int idInstalacion)
        {
            return this._instalaciones.mObtenConfiguracionReportesAutomaticos(idInstalacion);
        }

        public Task<bool> mGuardaConfiguracionReportesAutomaticos(int idInstalacion, CReporteAutomaticoConfig config)
        {
            if(config.UmbralFp < 0.00m || config.UmbralFp > 1.00m)
            {
                throw new ErrorAlCliente("El umbral de factor de potencia debe de estar entre 0 y 100 %.");
            }
            if(config.PorcentajeDap < 0.00m || config.PorcentajeDap > 1.00m)
            {
                throw new ErrorAlCliente("El porcentaje del Derecho Alumbrado Público debe de estar entre 0 y 100 %.");
            }
            if (config.FpDefault < 0.00m || config.FpDefault > 1.00m)
            {
                throw new ErrorAlCliente("El Factor de Potencia Default debe de estar entre 0 y 100 %.");
            }
            return this._instalaciones.mGuardaConfiguracionReportesAutomaticos(idInstalacion, config);
        }


        public async Task<ReporteDatos> mObtenDatosReporteMensual(int idInstalacion, int anno, int mes)
        {
            if (mes < 1 || mes > 12)
            {
                throw new ErrorAlCliente("El mes debe de estar entre 1 y 12.");
            }
            if (anno < 2000 || anno > DateTime.Now.Year)
            {
                throw new ErrorAlCliente("El año debe de estar entre 2000 y el año actual.");
            }

            var instalacion = this.mObtenInstalacionCompleta(idInstalacion);
            var cliente = instalacion.IdClienteNavigation;
            var reciboInfo = this._instalaciones.mObtenRecibosPorInstalacion(idInstalacion, anno)
                               .FirstOrDefault(r => r.MesRecibo.Year == anno && r.MesRecibo.Month == mes);
            if (reciboInfo == null)
            {
                throw new ErrorAlCliente("No se ha encontrado el Recibo para esta instalación en el mes y año seleccionados.");
            }
            if (instalacion.Zona == null)
            {
                throw new ErrorAlCliente("La instalación no tiene una zona asignada, por lo que no se pueden obtener las tarifas.");
            }
            if (instalacion.IdCatalogoTarifa == null)
            {
                throw new ErrorAlCliente("La instalación no tiene una tarifa asignada, por lo que no se pueden obtener las tarifas.");
            }

            var tarifas = this._interfaz.mObtenTarifasDivisiones(mes, anno, (int)instalacion.Zona, instalacion.IdCatalogoTarifa).FirstOrDefault();
            if (tarifas == null)
            {
                throw new ErrorAlCliente("No se han encontrado tarifas para la instalación en el mes y año seleccionados.");
            }

            var generacion = await this.mObtenGeneracionDiaria(cliente.IdCliente, idInstalacion, anno, mes);
            if (generacion == null)
            {
                throw new ErrorAlCliente("No se ha podido obtener la generación para la instalación en el mes y año seleccionados.");
            }

            var ahorroAcumulado = this._instalaciones.ObtenAhorroAcumulado(idInstalacion, anno, mes);
            if (ahorroAcumulado == null)
            {
                if (instalacion.InicioOperaciones.Value.Year == anno && instalacion.InicioOperaciones.Value.Month == mes)
                {
                    ahorroAcumulado = 0;
                }
                else
                {
                    throw new ErrorAlCliente("No se cuenta con datos de Ahorro acumulado del periodo anterior.");
                }
            }

            var configReporte = await this.mObtenConfiguracionReportesAutomaticos(idInstalacion);
            var umbralFactorPotencia = configReporte.UmbralFp ?? 0.90m;
            var factorPotenciaDefault = configReporte.FpDefault ?? 0.90m;
            var fechaMes = new DateOnly(anno, mes, 1);
            var diasFestivos = this._interfaz.mObtenDiasFestivos(anno).Where(d => d.Month == mes).Select(d => d.Day).ToArray();
            var modulo7Domingos = (8 - ((byte)fechaMes.DayOfWeek)) % 7;
            var generacionTotalFestivos = generacion.datos.Where(g => ((g.iDia % 7) == modulo7Domingos) || diasFestivos.Contains(g.iDia)).Sum(d => d.generacionTotal);
            var generacionTotal = generacion.datos.Sum(d => d.generacionTotal);
            var generacionTotalLaborables = generacionTotal - generacionTotalFestivos;
            var reciboKwh = (reciboInfo.KWhBase ?? 0) + (reciboInfo.KWhIntermedia ?? 0) + (reciboInfo.KWhPunta ?? 0);
            var reciboKW = (reciboInfo.KWbase ?? 0) + (reciboInfo.KWintermedia ?? 0) + (reciboInfo.KWpunta ?? 0);
            var demandaEquivalenteCPaneles = Math.Ceiling((double)reciboKwh / (24 * .57 * fechaMes.AddMonths(1).AddDays(-1).Day));
            var demandaEquivalenteSPaneles = Math.Ceiling((double)(reciboKwh + generacionTotal) / (24 * .57 * fechaMes.AddMonths(1).AddDays(-1).Day));
            var factorDePotencia = reciboKwh == 0 ? factorPotenciaDefault : (decimal)(1 / Math.Sqrt(1 + Math.Pow((double)((reciboInfo.ReactivosKvArh ?? 0) / reciboKwh), 2)));
            var factorDePotenciaConGeneracion = (reciboKwh + generacionTotal) == 0 ? factorPotenciaDefault : (decimal)(1 / Math.Sqrt(1 + Math.Pow((double)(reciboInfo.ReactivosKvArh ?? 0) / (double)(reciboKwh + generacionTotal), 2)));

            var repDatos = new ReporteDatos()
            {
                IdInstalacion = idInstalacion,
                iAnno = anno,
                iMes = mes,
                Cliente = instalacion.Nombre ?? cliente.Nombre ?? "",
                Direccion = instalacion.Ubicacion ?? "",
                RPU = instalacion.Rpu ?? "",
                Periodo = fechaMes.AddDays(-1).ToString("dd MMM yy") + " - " + new DateOnly(anno, mes + 1, 1).AddDays(-1).ToString("dd MMM yy"),
                NombreEnRecibo = configReporte.NombreEnRecibo ?? instalacion.Nombre ?? "",
                CapacidadInstalada = instalacion.PotenciaInstalada ?? 0,
                GeneracionPeriodo = generacionTotal,
                ConsumoTotalPorcentaje = 100
            };

            repDatos.ConsumoTotal = repDatos.GeneracionPeriodo + reciboKwh;
            repDatos.ConsumoPaneles = repDatos.GeneracionPeriodo;
            repDatos.ConsumoCFE = repDatos.ConsumoTotal - repDatos.ConsumoPaneles;
            repDatos.ConsumoCFEPorcentaje = repDatos.ConsumoTotal == 0 ? 0 : repDatos.ConsumoCFE / repDatos.ConsumoTotal * 100;
            repDatos.ConsumoPanelesPorcentaje = repDatos.ConsumoTotal == 0 ? 0 : repDatos.ConsumoPaneles / repDatos.ConsumoTotal * 100;

            #region Desglose recibo de luz
            repDatos.FijoCPaneles = tarifas.ValorOpSsb ?? 0;
            repDatos.FijoSPaneles = repDatos.FijoCPaneles;
            repDatos.FijoAhorro = repDatos.FijoSPaneles - repDatos.FijoCPaneles;

            repDatos.BaseCPaneles = (reciboInfo.KWhBase ?? 0) * (tarifas.ValorEnergiaBase ?? 0);
            repDatos.BaseSPaneles = (tarifas.ValorEnergiaBase ?? 0) * ((reciboInfo.KWhBase ?? 0) + generacionTotalFestivos);
            repDatos.BaseAhorro = repDatos.BaseSPaneles - repDatos.BaseCPaneles;

            repDatos.IntermediaCPaneles = (reciboInfo.KWhIntermedia ?? 0) * (tarifas.ValorEnergiaIntermedia ?? 0);
            repDatos.IntermediaSPaneles = (tarifas.ValorEnergiaIntermedia ?? 0) * ((reciboInfo.KWhIntermedia ?? 0) + generacionTotalLaborables);
            repDatos.IntermediaAhorro = repDatos.IntermediaSPaneles - repDatos.IntermediaCPaneles;

            repDatos.PuntaCPaneles = (reciboInfo.KWhPunta ?? 0) * (tarifas.ValorEnergiaPunta ?? 0);
            repDatos.PuntaSPaneles = repDatos.PuntaCPaneles;
            repDatos.PuntaAhorro = repDatos.PuntaSPaneles - repDatos.PuntaCPaneles;

            repDatos.TransmisionCPaneles = reciboKwh * (tarifas.ValorTransmision ?? 0);
            repDatos.TransmisionSPaneles = (tarifas.ValorTransmision ?? 0) * (reciboKwh + generacionTotal);
            repDatos.TransmisionAhorro = repDatos.TransmisionSPaneles - repDatos.TransmisionCPaneles;

            repDatos.CENACECPaneles = reciboKwh * (tarifas.ValorOpCenace ?? 0);
            repDatos.CENACESPaneles = (tarifas.ValorOpCenace ?? 0) * (reciboKwh + generacionTotal);
            repDatos.CENACEAhorro = repDatos.CENACESPaneles - repDatos.CENACECPaneles;

            repDatos.SCNMEMCPaneles = reciboKwh * (tarifas.ValorServiciosNoMem ?? 0);
            repDatos.SCNMEMSPaneles = (tarifas.ValorServiciosNoMem ?? 0) * (reciboKwh + generacionTotal);
            repDatos.SCNMEMAhorro = repDatos.SCNMEMSPaneles - repDatos.SCNMEMCPaneles;

            repDatos.DistribucionCPaneles = (decimal)(((double)(tarifas.ValorDistribucion ?? 0)) * Math.Min((double)(new[] { reciboInfo.KWbase, reciboInfo.KWintermedia, reciboInfo.KWpunta }.Max() ?? 0), demandaEquivalenteCPaneles));
            repDatos.DistribucionSPaneles = (decimal)(((double)(tarifas.ValorDistribucion ?? 0)) * Math.Min((double)(new[] { reciboInfo.KWbase, reciboInfo.KWintermedia, reciboInfo.KWpunta }.Max() ?? 0), demandaEquivalenteSPaneles));
            repDatos.DistribucionAhorro = repDatos.DistribucionSPaneles - repDatos.DistribucionCPaneles;

            repDatos.CapacidadCPaneles = (decimal)(((double)(tarifas.ValorCapacidad ?? 0)) * Math.Min((double)demandaEquivalenteCPaneles, (double)(reciboInfo.KWpunta ?? 0)));
            repDatos.CapacidadSPaneles = (decimal)(((double)(tarifas.ValorCapacidad ?? 0)) * Math.Min((double)demandaEquivalenteSPaneles, (double)(reciboInfo.KWpunta ?? 0)));
            repDatos.CapacidadAhorro = repDatos.CapacidadSPaneles - repDatos.CapacidadCPaneles;

            repDatos.EnergiaCPaneles = repDatos.BaseCPaneles + repDatos.IntermediaCPaneles + repDatos.PuntaCPaneles + repDatos.TransmisionCPaneles
                + repDatos.CENACECPaneles + repDatos.SCNMEMCPaneles + repDatos.DistribucionCPaneles + repDatos.CapacidadCPaneles;
            repDatos.EnergiaSPaneles = repDatos.BaseSPaneles + repDatos.IntermediaSPaneles + repDatos.PuntaSPaneles + repDatos.TransmisionSPaneles
                + repDatos.CENACESPaneles + repDatos.SCNMEMSPaneles + repDatos.DistribucionSPaneles + repDatos.CapacidadSPaneles;
            repDatos.EnergiaAhorro = repDatos.EnergiaSPaneles - repDatos.EnergiaCPaneles;

            repDatos.FactorPotenciaCPaneles = ((tarifas.ValorOpSsb ?? 0) + repDatos.EnergiaCPaneles) * Math.Round((decimal)(factorDePotencia >= umbralFactorPotencia ? -.25 : -.6) * (1 - umbralFactorPotencia / factorDePotencia), 3);
            repDatos.FactorPotenciaSPaneles = ((tarifas.ValorOpSsb ?? 0) + repDatos.EnergiaSPaneles) * Math.Round((decimal)(factorDePotenciaConGeneracion >= umbralFactorPotencia ? -.25 : -.6) * (1 - umbralFactorPotencia / factorDePotenciaConGeneracion), 3);
            repDatos.FactorPotenciaAhorro = repDatos.FactorPotenciaSPaneles - repDatos.FactorPotenciaCPaneles;

            repDatos.SubtotalCPaneles = repDatos.FijoCPaneles + repDatos.EnergiaCPaneles + repDatos.FactorPotenciaCPaneles;
            repDatos.SubtotalSPaneles = repDatos.FijoSPaneles + repDatos.EnergiaSPaneles + repDatos.FactorPotenciaSPaneles;
            repDatos.SubtotalAhorro = repDatos.SubtotalSPaneles - repDatos.SubtotalCPaneles;

            decimal sumaConceptosC = repDatos.FijoCPaneles
                + repDatos.BaseCPaneles
                + repDatos.IntermediaCPaneles
                + repDatos.PuntaCPaneles
                + repDatos.TransmisionCPaneles
                + repDatos.CENACECPaneles
                + repDatos.SCNMEMCPaneles
                + repDatos.DistribucionCPaneles;

            decimal sumaConceptosS = repDatos.FijoSPaneles
                + repDatos.BaseSPaneles
                + repDatos.IntermediaSPaneles
                + repDatos.PuntaSPaneles
                + repDatos.TransmisionSPaneles
                + repDatos.CENACESPaneles
                + repDatos.SCNMEMSPaneles
                + repDatos.DistribucionSPaneles;

            repDatos.BajaTension2C = Math.Round(sumaConceptosC * 0.02m, 2);
            repDatos.BajaTension2S = Math.Round(sumaConceptosS * 0.02m, 2);
            repDatos.BajaTension2Ahorro = repDatos.BajaTension2S - repDatos.BajaTension2C;

            bool bajaTension2Enabled = false;
            try
            {
                var prop = configReporte?.GetType().GetProperty("BajaTension2");
                if (prop != null)
                {
                    var val = prop.GetValue(configReporte);
                    bajaTension2Enabled = val is bool b && b;
                }
            }
            catch
            {
                bajaTension2Enabled = false;
            }

            if (bajaTension2Enabled)
            {
                repDatos.SubtotalCPaneles += repDatos.BajaTension2C;
                repDatos.SubtotalSPaneles += repDatos.BajaTension2S;
                repDatos.SubtotalAhorro = repDatos.SubtotalSPaneles - repDatos.SubtotalCPaneles;
            }

            repDatos.DAPCPaneles = (configReporte.PorcentajeDap ?? 0) * repDatos.SubtotalCPaneles;
            repDatos.DAPSPaneles = (configReporte.PorcentajeDap ?? 0) * repDatos.SubtotalSPaneles;
            repDatos.DAPAhorro = repDatos.DAPSPaneles - repDatos.DAPCPaneles;

            repDatos.IVACPaneles = repDatos.SubtotalCPaneles * 0.16m;
            repDatos.IVASPaneles = repDatos.SubtotalSPaneles * 0.16m;
            repDatos.IVAhorro = repDatos.IVASPaneles - repDatos.IVACPaneles;

            repDatos.TotalCPaneles = repDatos.SubtotalCPaneles + repDatos.DAPCPaneles + repDatos.IVACPaneles;
            repDatos.TotalSPaneles = repDatos.SubtotalSPaneles + repDatos.DAPSPaneles + repDatos.IVASPaneles;
            repDatos.TotalAhorro = repDatos.TotalSPaneles - repDatos.TotalCPaneles;

            repDatos.AhorroAcumuladoSIva = ahorroAcumulado.Value + repDatos.TotalAhorro - repDatos.IVAhorro;
            #endregion

            repDatos.PagoSinPaneles = repDatos.TotalSPaneles;
            repDatos.PagoConPaneles = repDatos.TotalCPaneles;
            repDatos.Ahorro = repDatos.PagoSinPaneles - repDatos.PagoConPaneles;
            repDatos.PorcentajeAhorro = repDatos.PagoSinPaneles == 0 ? 0 : repDatos.Ahorro / repDatos.PagoSinPaneles * 100;

            #region Aporte al medio ambiente

            repDatos.AporteArboles = Math.Round((double)generacionTotal * 101 / 3000).ToString() + " árboles.";
            repDatos.AporteCO2 = (int)Math.Round((double)generacionTotal * .505 / 1000);
            repDatos.AhorroAcumuladoDesde = (instalacion.InicioOperaciones ?? DateOnly.FromDateTime(DateTime.Now)).ToString("MMMM yyyy");

            #endregion

            #region Gráficas

            repDatos.PorcentajeCumplimiento = mObtenKpiAnual(anno, false, mes).FirstOrDefault(i => i.IdInstalacion == idInstalacion)?.Porcentaje ?? 0;
            repDatos.GeneracionDiaria = generacion.datos.Select(d => d.generacionTotal).ToArray();
            repDatos.HistoricoGenPVEsteAnnio = (await this.mObtenGeneracion(instalacion.IdCliente, idInstalacion, anno)).arrValoresReales.Take(mes).Select(v => v ?? 0).ToArray();
            repDatos.HistoricoGenPVAnnioAnterior = (instalacion.InicioOperaciones?.Year ?? 50000) >= anno ? null :
                (await this.mObtenGeneracion(instalacion.IdCliente, idInstalacion, anno - 1)).arrValoresReales.Select(v => v ?? 0).ToArray();

            repDatos.HistoricoConsumo = this._instalaciones.mObtenConsumoHistorico(idInstalacion, anno);
            repDatos.HistoricoFacturas = this._instalaciones.mObtenFacturasHistorico(idInstalacion, anno);

            #endregion

            return repDatos;
        }

    }

}
