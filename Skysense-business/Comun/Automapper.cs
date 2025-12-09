using AutoMapper;
using Skysense_models.DB;
using Skysense_models.DB.FunctionReturns;
using Skysense_persistencia.Entidades;

namespace Skysense_business.Comun
{
    public class Automapper:Profile
    {
        public Automapper() {
            CreateMap<Skysense_persistencia.Entidades.User, CUser>();
            CreateMap<Skysense_persistencia.Entidades.Role, CRole>();
            CreateMap<Skysense_persistencia.Entidades.Cliente, CCliente>()
                .ForMember(g => g.CuantasInstalaciones, opt => opt.MapFrom(src => src.Instalaciones != null ? src.Instalaciones.Count() : 0));
            CreateMap<Skysense_persistencia.Entidades.Instalacione, CInstalacione>()
                .ForMember(g => g.IdGrupo, opt => opt.NullSubstitute(0))
                .ForMember(g => g.GrupoNombre, opt => opt.MapFrom(src => src.IdGrupoNavigation != null ? src.IdGrupoNavigation.Nombre : "Independientes"));
            CreateMap<Skysense_persistencia.Entidades.Panele, CPanele>();
            CreateMap<Skysense_persistencia.Entidades.Inversor, CInversore>();
            CreateMap<Skysense_persistencia.Entidades.Plataforma, CPlataforma>();
            CreateMap<Skysense_persistencia.Entidades.CatalogoMaestro, CCatalogoMaestro>();
            CreateMap<Skysense_persistencia.Entidades.CatalogoOpcione, CCatalogoOpcione>();
            CreateMap<Skysense_persistencia.Entidades.Grupo, CGrupo>();
            CreateMap<Skysense_persistencia.Entidades.Documento, CDocumento>();
            CreateMap<Skysense_persistencia.Entidades.RecibosMensuale, CRecibo>();
            CreateMap<Skysense_persistencia.Entidades.ReportesMensuale, CReporte>();
            CreateMap<Skysense_persistencia.Entidades.InversorApiencabezado, CInversorApiencabezado>();
            CreateMap<Skysense_persistencia.Entidades.InversorApigeneracion, CInversorApigeneracion>();
            CreateMap<Skysense_persistencia.Entidades.InstalacionApigeneracionMensual, CInstalacionApigeneracionMensual>();
            CreateMap<Skysense_persistencia.Entidades.fObtenGeneracionMensualResult, CfObtenGeneracionMensualResult>();
            CreateMap<Skysense_persistencia.Entidades.fObtenKPIInstalacionesAnualMensualResult, KPIInstalacionesResult>();
            CreateMap<Skysense_persistencia.Entidades.TarifasDivisione, CTarifasDivisione>();
            CreateMap<Skysense_persistencia.Entidades.ReporteAutomaticoConfig, CReporteAutomaticoConfig>();

            CreateMap<CReporteAutomaticoConfig, Skysense_persistencia.Entidades.ReporteAutomaticoConfig>();


        }
    }
}
