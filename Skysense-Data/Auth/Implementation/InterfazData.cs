using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skysense_Data.Auth.Interfaces;
using Skysense_models.DB;
using Skysense_models.Errors;
using Skysense_models.Otros;
using Skysense_models.Peticiones;
using Skysense_persistencia.Entidades;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Skysense_Data.Auth.Implementation
{
    public class InterfazData : IInterfazData
    {
        Skysense_persistencia.Entidades.SkysenseDevContext _SkysenseDevContext;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        IMapper _mapper;

        public InterfazData(Skysense_persistencia.Entidades.SkysenseDevContext SkysenseDevContext, IMapper mapper, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._SkysenseDevContext = SkysenseDevContext;
            this._mapper = mapper;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public bool mActualizaCatalogoMaestro(CCatalogoOpcione opcionAModificar)
        {
            var catalogoOpcion = this._SkysenseDevContext.CatalogoOpciones
                .Where(c => c.IdCatalogo == opcionAModificar.IdCatalogo && c.IdOpcion == opcionAModificar.IdOpcion)
                .SingleOrDefault();

            if (catalogoOpcion == null)
            {
                catalogoOpcion = new CatalogoOpcione();
                catalogoOpcion.IdCatalogo = opcionAModificar.IdCatalogo;
                catalogoOpcion.IdOpcion = (this._SkysenseDevContext.CatalogoOpciones.Where(c => c.IdCatalogo == opcionAModificar.IdCatalogo).Max(c => (int?)c.IdOpcion) ?? 0) + 1;
                catalogoOpcion.NombreOpcion = opcionAModificar.NombreOpcion;
                catalogoOpcion.InfoAdicional = opcionAModificar.InfoAdicional;
                catalogoOpcion.Disponible = opcionAModificar.Disponible;

                this._SkysenseDevContext.CatalogoOpciones.Add(catalogoOpcion);
            }
            else
            {
                catalogoOpcion.NombreOpcion = opcionAModificar.NombreOpcion;
                catalogoOpcion.InfoAdicional = opcionAModificar.InfoAdicional;
                catalogoOpcion.Disponible = opcionAModificar.Disponible;
            }

            this._SkysenseDevContext.SaveChanges();

            return true;
        }

        public CCatalogoMaestro[] mObtenCatalogoMaestro(int[] idCatalogos)
        {
            return (from cm in this._SkysenseDevContext.CatalogoMaestros
                    where idCatalogos.Contains(cm.IdCatalogo)
                    select new CCatalogoMaestro()
                    {
                        Opciones = this._mapper.Map<CCatalogoOpcione[]>(cm.CatalogoOpciones.ToArray()),
                        IdCatalogo = cm.IdCatalogo,
                        Nombre = cm.Nombre,
                        InfoAdicional = cm.InfoAdicional,
                        Disponible = cm.Disponible
                    }).ToArray();

        }

        public CCatalogoMaestro[] mObtenCatalogosMaestros()
        {
            return this._mapper.Map<CCatalogoMaestro[]>(this._SkysenseDevContext.CatalogoMaestros.Where(c => c.Disponible == true).ToArray());
        }

        public bool mEliminaOpcionCatalogo(CCatalogoOpcione opcionAEliminar)
        {
            var opc = this._SkysenseDevContext.CatalogoOpciones.SingleOrDefault(c => c.IdCatalogo == opcionAEliminar.IdCatalogo && c.IdOpcion == opcionAEliminar.IdOpcion);

            if (opc == null)
            {
                throw new ErrorAlCliente("No se pudo encontrar el registro a eliminar");
            }

            this._SkysenseDevContext.CatalogoOpciones.Remove(opc);


            this._SkysenseDevContext.SaveChanges();
            return true;
        }




        public CGrupo[] mObtenGrupos()
        {
            return this._mapper.Map<CGrupo[]>(this._SkysenseDevContext.Grupos.ToArray());
        }

        public bool mEliminaGrupo(int opcionAEliminar)
        {
            var grp = this._SkysenseDevContext.Grupos.SingleOrDefault(c => c.IdGrupo == opcionAEliminar);
            if (grp == null)
            {
                throw new ErrorAlCliente("No se pudo encontrar el registro a eliminar");
            }

            Task.WaitAny(this._SkysenseDevContext.Instalaciones.Where(i => i.IdGrupo == opcionAEliminar).ForEachAsync(o => o.IdGrupo = null));

            this._SkysenseDevContext.Grupos.Remove(grp);

            this._SkysenseDevContext.SaveChanges();
            return true;
        }

        public bool mActualizaGrupo(CGrupo opcionAModificar)
        {
            var grupo = this._SkysenseDevContext.Grupos
                .Where(c => c.IdGrupo == opcionAModificar.IdGrupo)
                .SingleOrDefault();
            if (grupo == null)
            {
                return false;
            }
            grupo.Nombre = opcionAModificar.Nombre;
            grupo.Disponible = opcionAModificar.Disponible;
            this._SkysenseDevContext.SaveChanges();
            return true;
        }

        public string[] mObtenRolesSistema()
        {
            return this._roleManager.Roles.OrderBy(r=>r.Name).Select(r => r.Name)?.ToArray() ?? [];
        }
        public async Task<bool> mActualizaUsuario(UserTabla usuarioAModificar)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == usuarioAModificar.id);
            if (user == null)
            {
                throw new ErrorAlCliente("No se pudo encontrar el usuario a modificar");
            }
            user.UserName = usuarioAModificar.name;
            user.Email = usuarioAModificar.mail;
            //if (usuarioAModificar.modificaBloqueo == true)
            user.LockoutEnd = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now ? 
                (usuarioAModificar.bloqueado ? user.LockoutEnd : DateTimeOffset.Now) : (usuarioAModificar.bloqueado ? DateTimeOffset.MaxValue : (DateTimeOffset?)null);

            await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));


            if (!string.IsNullOrEmpty(usuarioAModificar.rol))
            {
                var role = await _roleManager.FindByNameAsync(usuarioAModificar.rol);
                if (role != null)
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                }
            }

            var resultado = await _userManager.UpdateAsync(user);

            return resultado.Succeeded;

        }
        public UserTabla[] mObtenUsuarios()
        {
            return _userManager.Users.Select(u => new UserTabla
            {
                id = u.Id,
                name = u.UserName ?? "",
                mail = u.Email!,
                bloqueado = u.LockoutEnabled && u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.Now,
                rol = _userManager.GetRolesAsync(u).Result.FirstOrDefault()?? ""
            }).ToArray();
        }

        public CTarifasDivisione[] mObtenTarifasDivisiones(int mes, int annio, int IdZona, int? idTarifa = null)
        {
            return this._mapper.Map<CTarifasDivisione[]>(this._SkysenseDevContext.TarifasDivisiones.Where(t => t.Mes == mes && t.Anno == annio && IdZona == t.IdDivision && (idTarifa == null || t.IdTarifa == idTarifa)).ToArray());
        }

        public bool mGuardaTarifasDivisiones(int iMes, int iAnno, int idZona, CTarifasDivisione[] tarifas)
        {
            var tarifasExistentes = this._SkysenseDevContext.TarifasDivisiones.Where(t => t.Mes == iMes && t.Anno == iAnno && t.IdDivision == idZona).ToList();
            foreach (var tarifa in tarifas)
            {
                var tarifaQ = tarifasExistentes.SingleOrDefault(t => t.IdTarifa == tarifa.IdTarifa);
                if (tarifaQ == null)
                {
                    tarifaQ = new TarifasDivisione()
                    {
                        IdDivision = idZona,
                        IdTarifa = tarifa.IdTarifa,
                        Mes = (byte)iMes,
                        Anno = (short)iAnno
                    };

                    this._SkysenseDevContext.TarifasDivisiones.Add(tarifaQ);
                }

                // Actualizar la tarifa existente
                tarifaQ.ValorTransmision = tarifa.ValorTransmision;
                tarifaQ.ValorDistribucion = tarifa.ValorDistribucion;
                tarifaQ.ValorEnergiaBase = tarifa.ValorEnergiaBase;
                tarifaQ.ValorEnergiaPunta = tarifa.ValorEnergiaPunta;
                tarifaQ.ValorEnergiaIntermedia = tarifa.ValorEnergiaIntermedia;
                tarifaQ.ValorEnergiaSemipunta = tarifa.ValorEnergiaSemipunta;
                tarifaQ.ValorCapacidad = tarifa.ValorCapacidad;
                tarifaQ.ValorOpCenace = tarifa.ValorOpCenace;
                tarifaQ.ValorOpSsb = tarifa.ValorOpSsb;
                tarifaQ.ValorServiciosNoMem = tarifa.ValorServiciosNoMem;
            }

            this._SkysenseDevContext.SaveChanges();
            return true;
        }


        public DateOnly[] mObtenDiasFestivos(int iAnno)
        {
            return this._SkysenseDevContext.DiasFestivos.Where(d => d.FechaFestiva.Year == iAnno).Select(f=>f.FechaFestiva).ToArray();
        }

        public bool mGuardaDiasFestivos(int iAnno,DateOnly[] dias)
        {

            foreach (var dia in dias)
            {
                var diaQ = this._SkysenseDevContext.DiasFestivos.SingleOrDefault(d => d.FechaFestiva == dia);
                if (diaQ == null)
                {
                    diaQ = new DiasFestivo();
                    this._SkysenseDevContext.DiasFestivos.Add(diaQ);
                }
                // Actualizar la tarifa existente
                diaQ.FechaFestiva = dia;
            }

            this._SkysenseDevContext.DiasFestivos.RemoveRange(
                this._SkysenseDevContext.DiasFestivos.Where(df => df.FechaFestiva.Year == iAnno && !dias.Contains(df.FechaFestiva))
                );
            this._SkysenseDevContext.SaveChanges();
            return true;
        }

    }
}
