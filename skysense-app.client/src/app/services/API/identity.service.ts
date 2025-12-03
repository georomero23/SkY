import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UsuarioModel } from '@models/usuarioModel';

@Injectable({
  providedIn: 'root'
})
export class IdentityService {

  controllerName:string = "Identity"
  baseURL:string;

  private http = inject(HttpClient);
  constructor() {
      this.baseURL="api/"+this.controllerName;
   }

   public mIntentarAccederAlSitio(auth: UsuarioModel):Observable<any>{
      return this.http.post<any>(this.baseURL+"/login?useCookies=true&useSessionCookies=true", {"email": auth.mail, "password": auth.cntrsn })
   }

   public mLogOut():Observable<any>{
      return this.http.post<any>(this.baseURL+"/logout", {})
   }

   public mRegistrarUsuario(correo: string, cntrsn: string):Observable<any> {
      return this.http.post<any>(this.baseURL+"/register", { "email": correo, "password": cntrsn });
   }

   public mOlvideContrasena(mail:string):Observable<any> {
      return this.http.post<any>(this.baseURL+"/forgotPassword", { "email": mail });
   }

   public mEstoyLogueadoObs(){
      this.http.get<any>(this.baseURL+"/manage/info");
   }
}
