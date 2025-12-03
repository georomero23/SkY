using Skysense_models;
using Skysense_models.DB;
using Skysense_models.DB.FunctionReturns;
using Skysense_models.Otros;
using Skysense_models.Respuestas;
using Skysense_models.Worker;
using Skysense_persistencia.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_Data.Auth.Interfaces
{
    public interface IInsatalaciones
    {
        public CInstalacione? mActualizaInstalacion(CInstalacione cInstalacion);
        public bool mEliminaInstalacion(CInstalacione cInstalacion);
        public Task<int> mInsertaNuevosPaneles(int idInstalacion, IEnumerable<CPanele> enumerable);
        public CInstalacione mNuevaInstalacion(CInstalacione cInstalacion);
        public Task<IEnumerable<CDatosInversoresAjustado>> mObtenDataInversoresAjustada(int idInstalacion, int iAnion);
        public Task<IEnumerable<CDatosInversoresAjustado>> mObtenDataInversoresAjustada(int idInstalacion, int iAnion, int iMes);
        public CInstalacione? mObtenInstalacion(int idCliente, int idInstalacion);
        public Paginacion<CInstalacione> mObtenInstalacionesPaginadas(int? idGrupo, string? busqueda, int paginaPaginacion, int registrosPorPagina);
        public CInstalacione[] mObtenInstalaciones(int IdCliente, int? IdInstalacion = null);
        public CPanele[] mObtenPaneles(int idInstalacion);
        public Paginacion<Skysense_models.Respuestas.Documento> mObtenDocumentos(int idInstalacion, int paginaPaginacion, int registrosPorPagina, int tipoDocumento);
        public Task<int> mInsertaModificaDocumento(CDocumento dcto);
        public CDocumento fObtenDocumento(int idInstalacion, int idDocumento);
        public Task mEliminaDocumento(int idInstalacion, int idDocumento);
        public CGrupo[] mObtenGrupos();
        void mModificaPanel(CPanele panele);
        void EliminarPanel(int idPanel);
        CReporte[] mObtenReportesPorInstalacion(int idInstalacion, int iAnnio);
        CRecibo[] mObtenRecibosPorInstalacion(int idInstalacion, int iAnnio);
        Task mInsertaRecibo(CRecibo cRecibo);
        Task<int> mInsertaReporte(CReporte cReporte);
        void mInsertaModificaReporte(CReporte reporte);
        CDocumento[] mObtenDocumentosTipo(int idInstalacion, int[] tipos);
        Paginacion<PanelSimple> mObtenPanelesPaginados(int pagina, int registrosPorPagina, string? buscador);
        OpcionesCatalogo[] mModificaAgregaCatalogo(int idCatalogoMaestro, int idOpcionCatalogo, string opcion);
        bool mCambiaGrupo(int idInstalacion, int idGrupoN);
        CInstalacione mObtenInstalacionCompleta(int idInstalacion);
        void mActualizaValoresGarantizados(int idInstalacion, int iAnnio, CInstalacionApigeneracionMensual[] valoresGarantizados);
        CInstalacionApigeneracionMensual[] mObtenValoresGarantizados(int idInstalacion, int iAnion);
        CfObtenGeneracionMensualResult[] mObtenValoresRealesDB(int idInstalacion, int iAnion);
        InversorDataDiaria[] mObtenGeneracionDiariaBD(int idInstalacion, int iAnio, int iMes);
        KPIInstalacionesResult[] mObtenKpiAnual(int anno, bool annoGarantia, int? mes);
        void mInsertaModificaRecibo(CRecibo recibo);
        decimal?[] mObtenConsumoHistorico(int idInstalacion, int anno);
        decimal?[][] mObtenFacturasHistorico(int idInstalacion, int anno);
        decimal? ObtenAhorroAcumulado(int idInstalacion, int anno, int mes);
        Task<CReporteAutomaticoConfig> mObtenConfiguracionReportesAutomaticos(int idInstalacion);
        Task<bool> mGuardaConfiguracionReportesAutomaticos(int idInstalacion, CReporteAutomaticoConfig config);
    }
}
