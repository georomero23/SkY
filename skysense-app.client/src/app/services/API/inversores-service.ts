import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { InversorDatosAjustados, InversorGeneracionDiaria, InversorModel, InversorSimple } from '@models/inversor-model';
import { ApiRespuestaModel, Paginacion } from '@models/apiRespuestaModel';


@Injectable({
  providedIn: 'root'
})
export class InversoresService {

  controllerName:string = "Inversores"
  baseURL:string;

  private http = inject(HttpClient);
  constructor() {
      this.baseURL="api/"+this.controllerName;
   }

   public mObtenerInversores(idInstalacion:number = 0):Observable<InversorModel[]>{
      return this.http.get<InversorModel[]>(this.baseURL+"/ObtenInversores/"+idInstalacion);
   }

   public mObtenerInversoresPaginados(pagina:number = 0, cuantos:number = 10, buscador: string|undefined):Observable<ApiRespuestaModel<Paginacion< InversorSimple>>>{
      return this.http.get<ApiRespuestaModel<Paginacion<InversorSimple>>>(this.baseURL+"/ObtenInversoresPaginados/"+pagina+"/"+cuantos,{params:{buscador:buscador??""}});
   }

   public mModificaInversor(idInstalacion:number, inversorModificado: InversorModel):Observable<ApiRespuestaModel<boolean>> {
      return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL+"/ModificaInserta/0/"+idInstalacion,[inversorModificado]);
   }

   public mAjustaDatosInversor(datosInversorModificado: InversorDatosAjustados[]):Observable<ApiRespuestaModel<boolean>> {
      return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL+"/AjustarDatosInversor",datosInversorModificado);
   }

   public mNuevosInversores(idInstalacion:number, inversoresNuevos: InversorModel[]):Observable<ApiRespuestaModel<boolean>> {
      return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL+"/ModificaInserta/1/"+idInstalacion, inversoresNuevos);
   }

   public mEliminarInversor(idInversor: number): Observable<ApiRespuestaModel<boolean>> {
      return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL+"/EliminarInversor/"+idInversor, {});
   }
}
