using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Skysense_Data.Auth.Interfaces;
using Skysense_models;
using Skysense_models.DB;
using Skysense_models.Errors;
using Skysense_models.Otros;
using Skysense_models.Respuestas;
using Skysense_persistencia.Entidades;
using System.Linq;

namespace Skysense_Data.Auth.Implementation
{
    public class InversorClass : IInversores
    {
        Skysense_persistencia.Entidades.SkysenseDevContext _SkysenseDevContext;
        IMapper _mapper;

        public InversorClass(Skysense_persistencia.Entidades.SkysenseDevContext SkysenseDevContext, IMapper mapper)
        {
            this._SkysenseDevContext = SkysenseDevContext;
            this._mapper = mapper;
        }

        public CInversore[] mObtenInversores(int idInstalacion)
        {
            return _mapper.Map<CInversore[]>(from c in _SkysenseDevContext.Inversors
                                             where idInstalacion == 0 || c.IdInstalacion == idInstalacion
                                             select c).ToArray();
        }

        public CInversore? mActualizaInversores(CInversore cInversor)
        {
            var inversor = _SkysenseDevContext.Inversors.Where(i => i.IdInversor == cInversor.IdInversor).SingleOrDefault();

            if (_SkysenseDevContext.Inversors.Any(i => i.IdInversor != cInversor.IdInversor && i.NumeroSerie == cInversor.NumeroSerie && i.Modelo == cInversor.Modelo))
            {
                throw new ErrorAlCliente("Se está intentando introducir un número de serie y modelo que otro Inversor.");
            }

            if (inversor != null)
            {
                inversor.NumeroSerie = cInversor.NumeroSerie;
                inversor.DegradacionAnual = cInversor.DegradacionAnual;
                inversor.Danado = cInversor.Danado;
                inversor.EsInicial = cInversor.EsInicial;
                inversor.Potencia = cInversor.Potencia;
                inversor.ProveedorIntermediario = cInversor.ProveedorIntermediario;
                inversor.ProveedorIntermediarioRfc = cInversor.ProveedorIntermediarioRfc;
                inversor.ProveedorSuministrador = cInversor.ProveedorSuministrador;
                inversor.ProveedorSuministradorRfc = cInversor.ProveedorSuministradorRfc;
                inversor.NumeroSerieReemplazo = cInversor.NumeroSerieReemplazo;
                inversor.Modelo = cInversor.Modelo;
                inversor.Marca = cInversor.Marca;

                _SkysenseDevContext.SaveChanges();

                return _mapper.Map<CInversore>(inversor);
            }

            return null;
        }

        public bool mEliminaInversor(CInversore cInversor)
        {
            var inversor = _SkysenseDevContext.Inversors.Where(c => c.IdInversor == cInversor.IdInversor).SingleOrDefault();
            if (inversor != null)
            {
                _SkysenseDevContext.Inversors.Remove(inversor);

                _SkysenseDevContext.SaveChanges();

                return true;
            }

            return false;
        }

        public CInversore[] mObtenInversor(CInversore cInstalacion)
        {
            throw new NotImplementedException();
        }

        public bool mNuevosInversores(int idInstalacion, CInversore[] cInversores)
        {
            for (int i = 0; i < cInversores.Length; i++)
            {
                for (int j = i+1; j < cInversores.Length; j++)
                {
                    if (cInversores[i].Modelo == cInversores[j].Modelo && cInversores[i].NumeroSerie == cInversores[j].NumeroSerie)
                    {
                        throw new ErrorAlCliente("Se están intentando introducir inversores con modelo y número de serie iguales.");
                    }
                }

                if(_SkysenseDevContext.Inversors.Any(inv=>cInversores[i].Modelo == inv.Modelo && cInversores[i].NumeroSerie == inv.NumeroSerie))
                {
                    throw new ErrorAlCliente("Se está intentando guardar un registro con modelo y número de serie ya existente.");
                }
            }


            _SkysenseDevContext.Inversors.AddRange(cInversores.Select(cInversores => new Inversor()
            {
                NumeroSerie = cInversores.NumeroSerie,
                DegradacionAnual = cInversores.DegradacionAnual,
                Danado = cInversores.Danado,
                NumeroSerieReemplazo = cInversores.NumeroSerieReemplazo,
                Potencia = cInversores.Potencia,
                IdInstalacion = idInstalacion,
                EsInicial = cInversores.EsInicial,
                ProveedorIntermediario = cInversores.ProveedorIntermediario,
                ProveedorIntermediarioRfc = cInversores.ProveedorIntermediarioRfc,
                ProveedorSuministrador = cInversores.ProveedorSuministrador,
                ProveedorSuministradorRfc = cInversores.ProveedorSuministradorRfc,
                Modelo = cInversores.Modelo,
                Marca = cInversores.Marca,
                IdEstadoInversor = 1, // Assuming 1 is the default state for a new inverter
            }));

            _SkysenseDevContext.SaveChanges();

            return true;
        }

        public async Task<bool> mAjustaDatosInversor(CDatosInversoresAjustado[] datos)
        {
            DateTime fecha = DateTime.Now;
            foreach (var dato in datos)
            {
                var invDato = this._SkysenseDevContext.InversorApiencabezados.Where(i => i.IdApi == dato.IdentificadorInversor).SingleOrDefault();

                if(invDato == null)
                {
                    throw new ErrorAlCliente($"No se encontró el inversor con identificador de API {dato.IdentificadorInversor}.");
                }

                var genInv = this._SkysenseDevContext.InversorApigeneracions.Where(g => g.IdInversorApi == invDato.IdInversorApi && DateOnly.FromDateTime(g.FechaValor) == dato.FechaRegistro).FirstOrDefault();

                if(genInv == null)
                {
                    this._SkysenseDevContext.InversorApigeneracions.Add(new InversorApigeneracion()
                    {
                        FechaUltimaActualizacion = fecha,
                        FechaValor = dato.FechaRegistro!.Value.ToDateTime(new TimeOnly(0,0)),
                        IdInversorApi = invDato.IdInversorApi,
                        Valor = dato.ValorNuevo??0,
                        ValorManual = true,
                        IdConsecutivo = (this._SkysenseDevContext.InversorApigeneracions.Where(g=> g.IdInversorApi == invDato.IdInversorApi).Max(g=> (int?)g.IdConsecutivo) ?? 0 ) + 1
                    });
                }
                else
                {
                    if (genInv.Valor == dato.ValorNuevo)
                    {
                        continue; // No hay cambio en el valor, se omite la actualización
                    }
                    genInv.Valor = dato.ValorNuevo ?? 0;
                    genInv.FechaUltimaActualizacion = fecha;
                    genInv.ValorManual = true;
                }

                await this._SkysenseDevContext.SaveChangesAsync();
            }

            return true;
        }

        public bool mEliminarInversor(int idInversor)
        {
            var inversor = _SkysenseDevContext.Inversors.Find(idInversor);
            if (inversor != null)
            {
                _SkysenseDevContext.Inversors.Remove(inversor);
                _SkysenseDevContext.SaveChanges();
                return true;
            }
            return false;
        }

        public Paginacion<InversorSimple> mObtenInversoresPaginados(int pagina, int registrosPorPagina, string? buscaddor)
        {
            var query = _SkysenseDevContext.Inversors.Include(i=>i.IdInstalacionNavigation).AsQueryable();

            if (buscaddor != null)
            {
                query = query.Where(i => i.NumeroSerie.Contains(buscaddor) ||
                                         i.Marca.Contains(buscaddor) ||
                                         i.Modelo.Contains(buscaddor) ||
                                         (i.IdInstalacionNavigation.Nombre != null && i.IdInstalacionNavigation.Nombre.Contains(buscaddor)));

            }

            var totalRegistros = query.Count();

            var inversores = query.Skip((pagina - 1) * registrosPorPagina)
                                   .Take(registrosPorPagina)
                                   .ToList();

            return new Paginacion<InversorSimple>
            {
                paginaActual = pagina,
                registrosPorPagina = registrosPorPagina,
                numeroPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina),
                masDatos = (pagina * registrosPorPagina) < totalRegistros,
                totalRegistros = totalRegistros,
                registros = inversores.Select(i=>new InversorSimple()
                {
                    idInstalacion = i.IdInstalacion,
                    idCliente = i.IdInstalacionNavigation.IdCliente,
                    instalacion = i.IdInstalacionNavigation.Nombre??"",
                    marca = i.Marca,
                    modelo = i.Modelo,
                    potencia = i.Potencia,
                    idInversor = i.IdInversor,
                    numeroSerie = i.NumeroSerie
                }).ToArray()
            };
        }
        
    }
}
