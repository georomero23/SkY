import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiRespuestaModel } from '@models/apiRespuestaModel';
import { BateriaTabla, OPCBateria, TagSelectOptions } from '@models/baterias-models';
import { OpcionesSelect } from '@models/catalogo-model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BateriasService {
  controllerName:string = "baterias"
  baseURL:string;

  private http = inject(HttpClient);
  constructor() {
      this.baseURL="api/"+this.controllerName;
   }

   public ObtenOpcionesTags(){
      return this.http.get<ApiRespuestaModel<TagSelectOptions[]>>(this.baseURL+"/ObtenTagsOpciones");
   }

   public ObtenConfiguracionBateria(idInstalacion:number){
      return this.http.get<ApiRespuestaModel<OPCBateria>>(this.baseURL+"/ObtenConfiguracionBateria/"+idInstalacion);
   }

   public GuardaConfiguracionBateria(bateria:OPCBateria){
      return this.http.post<ApiRespuestaModel<OPCBateria>>(this.baseURL+"/GuardaConfiguracionBateria/"+bateria.idInstalacion, bateria);
   }

   public ObtenTablaBaterias(busqueda:string = 'a'):Observable<ApiRespuestaModel<BateriaTabla>>{
      return this.http.get<ApiRespuestaModel<BateriaTabla>>(this.baseURL+"/ObtenTablaBaterias", {params: {busqueda}});
  }
}
