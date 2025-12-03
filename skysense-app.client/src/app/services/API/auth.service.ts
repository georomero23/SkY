import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UsuarioModel } from '@models/usuarioModel';
import { ApiRespuestaModel } from '@models/apiRespuestaModel';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  controllerName:string = "auth"
  baseURL:string;

  private http = inject(HttpClient);
  constructor() {
      this.baseURL="api/"+this.controllerName;
   }

   public mObtenInfoUsuarioLogueado():Observable<ApiRespuestaModel<UsuarioModel>>{
      return this.http.get<ApiRespuestaModel<UsuarioModel>>(this.baseURL+"/Me")
   }
}
