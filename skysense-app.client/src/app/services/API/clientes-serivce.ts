import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ClienteModel } from '@models/cliente-model';
import { ClienteDashboard, InstalacionDashboard } from '@models/dashboard-models';
import { ApiRespuestaModel, Paginacion } from '@models/apiRespuestaModel';


@Injectable({
  providedIn: 'root'
})
export class ClientesService {

  controllerName:string = "Clientes"
  baseURL:string;

  private http = inject(HttpClient);
  this: any;
  constructor() {
      this.baseURL="api/"+this.controllerName;
   }

   public mObtenerClientes(numeroPagina: number, registrosPorPagina: number, busqueda: string|undefined = ""){
      return this.http.get<ApiRespuestaModel<Paginacion<ClienteModel>>>(this.baseURL+"/ObtenClientes", {
         params: {
            numeroPagina: numeroPagina,
            registrosPorPagina: registrosPorPagina,
            busqueda: busqueda
         }
      });
   }

   public mModificaCliente(clienteModificado: ClienteModel):Observable<ApiRespuestaModel<ClienteModel>> {
      return this.http.post<ApiRespuestaModel<ClienteModel>>(this.baseURL+"/ModificaCliente",clienteModificado);
   }

   public mObtenerCliente(idCliente: number) {
      return this.http.get<ClienteDashboard>(this.baseURL+"/Cliente/"+idCliente);
   }

   public mObtenerInstalacionDelCliente(idCliente: number, idInstalacion:number) {
      return this.http.get<InstalacionDashboard>(this.baseURL+"/Instalacion/"+idCliente+"/"+idInstalacion);
   }

   public mNuevoCliente(cliente: ClienteModel) {
      return this.http.post<ApiRespuestaModel<ClienteModel>>(this.baseURL+"/NuevoCliente",cliente);
   }

   public mFusionarClientes(cliente: ClienteModel) {
      return this.http.post<ApiRespuestaModel<void>>(this.baseURL+"/FusionarClientes", cliente );
   }
}
