import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiRespuestaModel } from '@models/apiRespuestaModel';
import { CatalogoEncabezadoModel, CatalogoModel, ZonaTarifaModel } from '@models/catalogo-model';
import { GrupoModel } from '@models/instalacion-model';
import { UserTableModel } from '@models/usuarioModel';

@Injectable({
  providedIn: 'root'
})
export class InterfazService {
  controllerName:string = "Interfaz"
  baseURL:string;

  private http = inject(HttpClient);
  constructor() {
      this.baseURL="api/"+this.controllerName;
   }
   
  obtenerCatalogosMaestros() {
    return this.http.get<ApiRespuestaModel<CatalogoEncabezadoModel[]>>(this.baseURL + "/ObtenCatalogoDeCatalogos");
  }

  obtenerOpcionesCatalogos(...idCatalogos: number[]) {
    return this.http.get<ApiRespuestaModel<CatalogoEncabezadoModel[]>>(this.baseURL + "/ObtenCatalogosConOpciones/" + idCatalogos.join(","));
  }

  guardarCatalogo(catalogo: CatalogoModel) {
    return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL + "/ModificaCatalogo", catalogo);
  }

  eliminarCatalogo(catalogo: CatalogoModel) {
    return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL + "/EliminaOpcionCatalogo", catalogo);
  }
  
  obtenerGrupos() {
    return this.http.get<ApiRespuestaModel<GrupoModel[]>>(this.baseURL + "/ObtenGrupos");
  }

  guardarGrupo(grupo: GrupoModel) {
    return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL + "/ModificaGrupo", grupo);
  }

  eliminarGrupo(grupo: number) {
    return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL + "/EliminaGrupo", { id: grupo });
  }
  
  obtenerRolesSistema() {
    return this.http.get<ApiRespuestaModel<string[]>>(this.baseURL + "/ObtenRolesSistema");
  }
  obtenerUsuarios() {
    return this.http.get<ApiRespuestaModel<UserTableModel[]>>(this.baseURL + "/ObtenUsuarios");
  }

  modificarUsuario(usuario: UserTableModel) {
    return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL + "/ModificaUsuario", usuario);
  }

  guardarZonaTarifa(anoSeleccionado: number, mesSeleccionado: number, division:number, zonaTarifaEditando: ZonaTarifaModel[]) {
    return this.http.post<ApiRespuestaModel<boolean>>(`${this.baseURL}/GuardaZonaTarifa/${anoSeleccionado}/${mesSeleccionado}/${division}`, zonaTarifaEditando);
  }

  obtenerZonasTarifas(anoSeleccionado: number, mesSeleccionado: number, zona: number, tarifa: number | null = null) {
    return this.http.get<ApiRespuestaModel<ZonaTarifaModel[]>>(this.baseURL + "/ObtenZonasTarifas/" + anoSeleccionado + "/" + mesSeleccionado + "/" + zona + (tarifa !== null ? "/" + tarifa : ''));
  }
  
  getDiasFestivos(year: number) {
    return this.http.get<ApiRespuestaModel<Date[]>>(this.baseURL + "/ObtenDiasFestivos/" + year);
  }

  guardarDiasFestivos(year:number,fechasFestivas: Date[]) {
    return this.http.post<ApiRespuestaModel<boolean>>(this.baseURL + "/GuardaDiasFestivos/" + year, fechasFestivas.map(fecha => fecha.toISOString().split('T')[0]));
  }
}
