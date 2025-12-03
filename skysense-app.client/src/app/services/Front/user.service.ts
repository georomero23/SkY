import { inject, Injectable } from '@angular/core';
import { UsuarioModel } from '@models/usuarioModel';
import { IdentityService } from '@services/API/identity.service';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  _authService = inject(IdentityService);

  private _Usuario: UsuarioModel|undefined;
  public _Roles: string[]|undefined;

  constructor() { 
    this._Usuario = new UsuarioModel();
  }
  
   public mEstoyLogueadoLocal(){
      return !!window.localStorage.getItem('EtyLgd');
   }

   public get Usuario(){
      return this._Usuario;
   }

   public get Roles(){
      if(!this._Roles && 
         this.mEstoyLogueadoLocal() && 
         window.localStorage.getItem("Skylink") != null)
      {
         this._Roles = atob(window.localStorage.getItem("Skylink")!).split(",",1000);
         //console.log("Poniendo Roles: ",this.Roles)
      }
      return this._Roles;
   }
   
   public set Roles(r){
      this._Roles = r;
   }

   public mLoguearLocal(usuario: UsuarioModel){
      window.localStorage.setItem('EtyLgd', "1");
      this._Usuario = usuario;
      this._Roles = usuario.roles;
      window.localStorage.setItem('Skylink', btoa(this._Usuario.roles.join(",")));
   }

   public Desconectar(){
      window.localStorage.clear();
      this._Usuario = undefined;
      this._Roles = undefined;
   }
}
