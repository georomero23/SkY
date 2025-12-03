using Microsoft.AspNetCore.Http;
using Skysense_models;
using Skysense_models.DB;
using Skysense_models.DB.FunctionReturns;
using Skysense_models.Otros;
using Skysense_models.Peticiones;
using Skysense_models.Respuestas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_business.Auth.Interfaces
{
    public interface IInstalaciones
    {
        public Paginacion<CInstalacione> mObtenInstalaciones(int? idGrupo, string? busqueda, int paginaPaginacion, int registrosPorPagina);
        public CInstalacione? mActualizaInstalacion(CInstalacione cInstalacion);
        public bool mEliminaInstalacion(CInstalacione cInstalacion);
        public CInstalacione[] mObtenInstalacion(int idCliente, int? idInstalacion = null);
        public CInstalacione mNuevaInstalacion(CInstalacione cInstalacion);
        public Task<TablaGeneracion> mObtenGeneracion(int idCliente, int idInstalacion, int iAnio);
        public Task<TablaGeneracionDiaria> mObtenGeneracionDiaria(int idCliente, int idInstalacion, int iAnio, int iMes);
        public Task<MethodResponse<int>> mInsertaNuevosPaneles(int idInstalacion, IEnumerable<CPanele> paneles);
        public CPanele[] mObtenPaneles(int idInstalacion);
        public Paginacion<Documento> mObtenDocumentos(int idInstalacion, int paginaPaginacion, int registrosPorPagina, int tipoDocumento);
        Task<Documento> mInsertaModificaDocumento(Documento dcto, IFormFile archivo, DateOnly? iAnno);
        public Task mEliminaDocumento(int idInstalacion, int idDocumento);
        CDocumento mObtenDocumento(int idInstalacion, int idDocumento);
        public CGrupo[] mObtenGrupos();
        void mModificaPanel(CPanele panele);
        void EliminarPanel(int idPanel);
        CReporte[] mObtenReportesPorInstalacion(int idInstalacion, int iAnnio);
        CRecibo[] mObtenRecibosPorInstalacion(int idInstalacion, int iAnnio);
        void mInsertaModificaReporte(CReporte reporte);
        CDocumento[] mObtenDocumentosTipo(int idInstalacion, int[] tipos);
        Paginacion<PanelSimple> mObtenPanelesPaginados(int pagina, int registrosPorPagina, string? buscador);
        OpcionesCatalogo[] mModificaAgregaCatalogo(int idCatalogoMaestro, int idOpcionCatalogo, string opcion);
        bool mCambiaGrupo(int idInstalacion, int idGrupoN);
        CInstalacione mObtenInstalacionCompleta(int idInstalacion);
        void mActualizaValoresGarantizados(int idInstalacion, int iAnnio, CInstalacionApigeneracionMensual[] valoresGarantizados);
        KPIInstalacionesResult[] mObtenKpiAnual(int anno, bool annoGarantia, int? mes);
        void mInsertaModificaRecibo(CRecibo recibo);
        Task<ReporteDatos> mObtenDatosReporteMensual(int idInstalacion, int anno, int mes);
        Task mSubeReporteAutomatico(int idInstalacion, IFormFile archivo, DateOnly dateOnly, decimal panelGeneracion, decimal ahorroAcumulado, decimal ahorroAmbiental, decimal consumoCFE);
        Task<CReporteAutomaticoConfig> mObtenConfiguracionReportesAutomaticos(int idInstalacion);
        Task<bool> mGuardaConfiguracionReportesAutomaticos(int idInstalacion, CReporteAutomaticoConfig config);
    }
}
