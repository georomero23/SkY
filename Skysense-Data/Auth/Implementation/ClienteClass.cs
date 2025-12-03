using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Skysense_Data.Auth.Interfaces;
using Skysense_models.DB;
using Skysense_models.Errors;
using Skysense_models.Respuestas;
using Skysense_persistencia.Entidades;
using System.Linq;

namespace Skysense_Data.Auth.Implementation
{
    public class ClienteClass : ICliente
    {
        Skysense_persistencia.Entidades.SkysenseDevContext _SkysenseDevContext;
        IMapper _mapper;

        public ClienteClass(Skysense_persistencia.Entidades.SkysenseDevContext SkysenseDevContext, IMapper mapper)
        {
            this._SkysenseDevContext = SkysenseDevContext;
            this._mapper = mapper;
        }

        public Paginacion<CCliente> mObtenClientes(int idPagina, int cuantosRegistros, string? busqueda)
        {

            var res = this._mapper.Map<CCliente[]>((from i in _SkysenseDevContext.Clientes
                                                         where 
                                                         busqueda == null ||
                                                         (i.Nombre != null && i.Nombre.Contains(busqueda)) ||
                                                         (i.Rfc != null && i.Rfc.Contains(busqueda))
                                                         select i).Include(i => i.Instalaciones).ToArray());
            

            return new Paginacion<CCliente>()
            {
                paginaActual = idPagina,
                masDatos = res.Length == cuantosRegistros + 1,
                numeroPaginas = cuantosRegistros == 0 ? -1 :(int)Math.Ceiling((res.Length / (decimal)cuantosRegistros)),
                totalRegistros = res.Count(),
                registrosPorPagina = cuantosRegistros,
                registros = res.Skip(cuantosRegistros * (idPagina - 1)).Take(cuantosRegistros).ToArray()
            };
        }

        public CCliente? mActualizaCliente(CCliente cCliente)
        {
            if (string.IsNullOrEmpty(cCliente.Rfc))
            {
                throw new ErrorAlCliente("El RFC es obligatorio.", 101);
            }

            if(_SkysenseDevContext.Clientes.Any(c=> c.IdCliente != cCliente.IdCliente && c.Rfc == cCliente.Rfc))
            {
                throw new ErrorAlCliente("El RFC ya existe.", 102);
            }

            var cliente = _SkysenseDevContext.Clientes.Where(c => c.IdCliente == cCliente.IdCliente).SingleOrDefault();
            if(cliente != null)
            {
                cliente.Telefono = cCliente.Telefono;
                cliente.Correo = cCliente.Correo;
                cliente.Nombre = cCliente.Nombre;
                cliente.EsEspecial = cCliente.EsEspecial;
                cliente.Rfc = cCliente.Rfc;
                cliente.Contacto = cCliente.Contacto;

                _SkysenseDevContext.SaveChanges();

                return _mapper.Map<CCliente>(cliente);
            }

            return null;
        }

        public bool mEliminaCliente(CCliente cCliente)
        {
            var cliente = _SkysenseDevContext.Clientes.Where(c => c.IdCliente == cCliente.IdCliente).SingleOrDefault();
            if (cliente != null)
            {
                cliente.IndicadorEstado = 0;

                _SkysenseDevContext.SaveChanges();

                return true;
            }

            return false;
        }
        public CCliente[] mObtenClientes(CCliente cCliente)
        {

            return _mapper.Map<CCliente[]>(from c in _SkysenseDevContext.Clientes
                                           where c.IndicadorEstado == 1 &&
                                           (cCliente.IdCliente == 0 || (c.IdCliente == cCliente.IdCliente)) &&
                                           EF.Functions.Like(c.Telefono??"", "%" + cCliente.Telefono??"" + "%") &&
                                           EF.Functions.Like(c.Correo??"", "%" + cCliente.Correo??"" + "%") &&
                                           EF.Functions.Like(c.Nombre, "%" + cCliente.Nombre + "%")
                                           select c).ToArray();
        }
        public CCliente mNuevoCliente(CCliente cCliente)
        {

            if (string.IsNullOrEmpty(cCliente.Rfc))
            {
                throw new ErrorAlCliente("El RFC es obligatorio.", 101);
            }

            if (_SkysenseDevContext.Clientes.Any(c => c.IdCliente != cCliente.IdCliente && c.Rfc == cCliente.Rfc))
            {
                throw new ErrorAlCliente("El RFC ya existe.", 102);
            }

            var nuevoCliente = new Skysense_persistencia.Entidades.Cliente()
            {
                IdCliente = _SkysenseDevContext.Clientes.Max(i => i.IdCliente) + 1,
                Nombre = cCliente.Nombre,
                Correo = cCliente.Correo,
                IndicadorEstado = 1,
                EsEspecial = cCliente.EsEspecial,
                Rfc = cCliente.Rfc,
                Telefono = cCliente.Telefono,
                Contacto = cCliente.Contacto
            };
            
            _SkysenseDevContext.Clientes.Add(nuevoCliente);
            _SkysenseDevContext.SaveChanges();

            return _mapper.Map<CCliente>(nuevoCliente);
            
        }

        public async Task<string> mFusionaClientes(int idClienteOrigen, string rfcDestino)
        {
            var textp = "";
            var clienteDestino = this._SkysenseDevContext.Clientes.First(c => c.Rfc == rfcDestino);

            //Se le pasan todas sus instalaciones al nuevo cliente
            await this._SkysenseDevContext.Instalaciones.Where(i => i.IdCliente == idClienteOrigen).ForEachAsync(i => i.IdCliente = clienteDestino.IdCliente);
            await this._SkysenseDevContext.SaveChangesAsync();

            //El cliente anterior es eliminado
            var clienteOrigen = this._SkysenseDevContext.Clientes.First(c => c.IdCliente == idClienteOrigen);

            textp = $"Se han fusionado el cliente {clienteOrigen.Nombre} hacia el cliente {clienteDestino.Nombre}.";

            this._SkysenseDevContext.Clientes.Remove(clienteOrigen);
            await this._SkysenseDevContext.SaveChangesAsync();

            return textp;
        }

    }
}
