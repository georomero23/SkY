import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GrupoModel, InstalacionModel } from '@models/instalacion-model';
import { InstalacionGeneracionMensual, TablaGeneracion, TablaGeneracionDiaria } from '@models/dashboard-models';
import { PanelModel, PanelSimple } from '@models/panel-model';
import { ApiRespuestaModel, Paginacion } from '@models/apiRespuestaModel';
import { Documento, KPIInstalaciones, ReciboMensual, ReporteMensual } from '@models/tablas-model';
import { ReporteAutomaticoConfig, ReporteAutomaticoDatosModel } from '@models/reporteAutomatico-model';


@Injectable({
  providedIn: 'root'
})
export class InstalacionesService {

  _UltimaSeleccion: KPIInstalaciones | null = null;

  controllerName:string = "Instalaciones"
  baseURL:string;

  private http = inject(HttpClient);
  constructor() {
      this.baseURL="api/"+this.controllerName;
   }

   public mObtenerInstalaciones(
      paginaActual: number,
      registrosPorPagina: number,
      busqueda:string = "",
      idGrupo = -1
   ): Observable<ApiRespuestaModel<Paginacion<InstalacionModel>>>  {
      const params = {
         idGrupo: idGrupo.toString(),
         busqueda: busqueda
      };
      return this.http.get<ApiRespuestaModel<Paginacion<InstalacionModel>>>(
         `${this.baseURL}/ObtenInstalaciones/${paginaActual}/${registrosPorPagina}`,
         { params }
      );
   }

   public mModificaInstalacion(instalacionModificado: InstalacionModel):Observable<ApiRespuestaModel<InstalacionModel>> {
      return this.http.post<ApiRespuestaModel<InstalacionModel>>(this.baseURL+"/ModificaInstalacion",instalacionModificado);
   }

   public mObtenEstadisticas(idCliente: number, idInstalacion: number):Observable<any>{
      return this.http.get<any>(this.baseURL+"/Estadisticas/Tags/"+idCliente + "/"+idInstalacion);
   }

   public mObtenerTablaGeneracion(idCliente:number, idInstalacion: number, iAnio: number):Observable<TablaGeneracion>{
      return this.http.get<TablaGeneracion>(this.baseURL+"/Estadisticas/Generacion/"+idCliente + "/"+idInstalacion+ "/"+iAnio);
   }

   public mObtenerDatosDiarios(idCliente:number, idInstalacion: number, iAnio: number, iMes:number):Observable<TablaGeneracionDiaria>{
      return this.http.get<TablaGeneracionDiaria>(this.baseURL+"/Estadisticas/GeneracionDiaria/"+idCliente + "/"+idInstalacion+ "/"+iAnio+ "/"+iMes);
   }

   public mInsertaPaneles(idInstalacion:number, paneles: PanelModel[]):Observable<ApiRespuestaModel<boolean>> {
      return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL+"/Paneles/Insertar/"+idInstalacion, paneles);
   }

   public mObtenPaneles(idInstalacion:number):Observable<ApiRespuestaModel<PanelModel[]>> {
      return this.http.get<ApiRespuestaModel<PanelModel[]>>(this.baseURL+"/Paneles/"+idInstalacion);
   }

   public mObtenerDocumentos(idInstalacion:number, paginaPaginacion: number, registrosPorPagina:number, tipoDocumento: number):
      Observable<ApiRespuestaModel<Paginacion<Documento>>>{
      return this.http.get<ApiRespuestaModel<Paginacion<Documento>>>(this.baseURL+"/Documentos/" + idInstalacion + "/" + 
         paginaPaginacion + "/" + registrosPorPagina + "/" + tipoDocumento);
   }

   public mObtenerDocumentosTipos(idInstalacion:number, tipos:number[]):
      Observable<ApiRespuestaModel<Documento[]>>{
      return this.http.get<ApiRespuestaModel<Documento[]>>(this.baseURL+"/DocumentosTipo/" + idInstalacion + "/" + tipos.join(','));
   }

   public mModificaDocumento(formData: any):Observable<ApiRespuestaModel<Paginacion<Documento>>>{
      return this.http.post<ApiRespuestaModel<Paginacion<Documento>>>(this.baseURL+"/Documento", formData);
   }

   public mModificaInfoReporte(reporte:ReporteMensual):Observable<ApiRespuestaModel<boolean>>{
      const objeto = {...reporte,mesReporte:reporte.mesReporte.toISOString().split('T')[0]}; // Convertir a formato 'YYYY-MM-DD'
      return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL+"/Reporte", objeto);
   }

   public mModificaInfoRecibo(recibo:ReciboMensual):Observable<ApiRespuestaModel<boolean>>{
      const objeto = {...recibo,mesRecibo:recibo.mesRecibo.toISOString().split('T')[0]}; // Convertir a formato 'YYYY-MM-DD'
      return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL+"/Recibo", objeto);
   }

   public mEliminaDocumento(idInstalacion:number, idDocumento:number):Observable<ApiRespuestaModel<null>>{
      return this.http.post<ApiRespuestaModel<null>>(this.baseURL+"/DocumentoRemove/" + idInstalacion + "/" + idDocumento, {});
   }

   public descargarDocumento(idInstalacion: number, idDocumento: number): Observable<Blob> {
      return this.http.get(`${this.baseURL}/DocumentoDownload/${idInstalacion}/${idDocumento}`, { responseType: 'blob' });
   }

   public mObtenerGrupos(): Observable<ApiRespuestaModel<GrupoModel[]>> {
      return this.http.get<ApiRespuestaModel<GrupoModel[]>>(`${this.baseURL}/Grupos`);
   }

   public mModificaGrupo(idInstalacion: number, idGrupoNuevo : number):Observable<boolean> {
      return this.http.post<boolean>(this.baseURL+"/ModificaGrupo/"+idInstalacion + "/" + idGrupoNuevo, { });
   }

   public mModificaPanel(arg0: PanelModel) {
     return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL+"/Paneles/Modificar", arg0);
   }

   public mEliminarPanel(idAparato: number) {
      return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL+"/Paneles/Eliminar/"+idAparato, {});
   }

   public mObtenerReportes(idInstalacion: number, iAnnio: number): Observable<ApiRespuestaModel<ReporteMensual[]>> {
      return this.http.get<ApiRespuestaModel<ReporteMensual[]>>(`${this.baseURL}/Reportes/${idInstalacion}/${iAnnio}`);
   }

   public mObtenerRecibos(idInstalacion: number, iAnnio: number): Observable<ApiRespuestaModel<ReciboMensual[]>> {
      return this.http.get<ApiRespuestaModel<ReciboMensual[]>>(`${this.baseURL}/Recibos/${idInstalacion}/${iAnnio}`);
   }

   public mObtenerPanelesPaginados(pagina:number = 0, cuantos:number = 10, buscador: string|undefined):Observable<ApiRespuestaModel<Paginacion<PanelSimple>>>{
      return this.http.get<ApiRespuestaModel<Paginacion<PanelSimple>>>(this.baseURL+"/ObtenPanelesPaginados/"+pagina+"/"+cuantos,{params:{buscador:buscador??""}});
   }

   public mObtenerInstalacion(idInstalacion: number) {
    return this.http.get<ApiRespuestaModel<InstalacionModel>>(`${this.baseURL}/Instalacion/${idInstalacion}`);
  }

   public mNuevaInstalacion(inst: InstalacionModel) {
      return this.http.post<ApiRespuestaModel<InstalacionModel>>(this.baseURL+"/NuevaInstalacion", inst);
   }


  public mActualizarGarantizados(idInstalacion: number, iAnio: number, valoresGarantizados: InstalacionGeneracionMensual[]) {
    return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL+`/Garantizados/${idInstalacion}/${iAnio}`, valoresGarantizados);
  }

  
  public ObtenKPIInstalaciones(annoGarantia: boolean, valorAnno: number, valorMes: number|null) {
    return this.http.get<ApiRespuestaModel<KPIInstalaciones[]>>(`${this.baseURL}/ObtenKPIInstalaciones/${valorAnno}/${annoGarantia}${valorMes !== null ? '/' + valorMes : ''}`);
  }

  
  public obtenerReporteAutomatico(idInstalacion: number, iAnno: number, iMes: number) {
    return this.http.get<ApiRespuestaModel<ReporteAutomaticoDatosModel>>(`${this.baseURL}/ObtenDatosReporteMensual/${idInstalacion}/${iAnno}/${iMes}`);
  }

  
  public mGuardaReporteAutomatico(formData: FormData, idInstalacion: number) {
    return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL+"/GuardaReporteAutomatico/"+idInstalacion, formData);
  }

  public mObtenConfigReporteAutomatico(idInstalacion: number) {
    return this.http.get<ApiRespuestaModel<ReporteAutomaticoConfig>>(`${this.baseURL}/ConfiguracionReportesAutomaticos/${idInstalacion}`);
  }

  public mGuardaConfiguracionReporteAutomatico(idInstalacion: number, configRA: ReporteAutomaticoConfig) {
    return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL+"/ConfiguracionReportesAutomaticos/"+idInstalacion, configRA);
  }
}
