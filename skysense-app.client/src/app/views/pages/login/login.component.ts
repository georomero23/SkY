import { Component, inject, OnInit, QueryList, viewChild, ViewChildren } from '@angular/core';
import { CommonModule, NgStyle } from '@angular/common';
import { IconDirective } from '@coreui/icons-angular';
import { ContainerComponent, RowComponent, ColComponent, CardGroupComponent, TextColorDirective, CardComponent, CardBodyComponent, FormDirective, InputGroupComponent, InputGroupTextDirective, FormControlDirective, ButtonDirective, AlertComponent, SpinnerComponent, CardHeaderComponent } from '@coreui/angular';
import { IdentityService } from '@services/API/identity.service';
import { UsuarioModel } from '@models/usuarioModel';
import { ActivatedRoute, Router } from '@angular/router';
import { UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { AppToastComponent } from '../../notifications/toasters/toast-simple/toast.component';
import { filter, map, Observable, Subscription, switchMap } from 'rxjs';
import { UserService } from '@services/Front/user.service';
import { ToastService } from '@services/Front/toast.service';
import { ToastModel } from '@models/toast-model';
import { animate, style, transition, trigger } from '@angular/animations';
import { AuthService } from '@services/API/auth.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
    imports: [CommonModule,ContainerComponent, RowComponent, ColComponent, CardGroupComponent, TextColorDirective, 
      CardComponent, CardBodyComponent, FormDirective, InputGroupComponent, InputGroupTextDirective, 
      IconDirective, FormControlDirective, ButtonDirective, NgStyle, AlertComponent, SpinnerComponent, CardHeaderComponent],
    animations: [
      trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: 0 }), // Start with opacity 0 (invisible)
        animate('1500ms ease-in', style({ opacity: 1 })) // Animate to opacity 1 (visible) over 500ms with ease-in
      ])
    ])
    ]
})
export class LoginComponent implements OnInit {

  _routeService:Router = inject(Router)
  _indentityService:IdentityService = inject(IdentityService);
  _userService:UserService = inject(UserService);
  _toastService = inject(ToastService);
  _iniciandoSesion:boolean = false;

  public _intentoFallido:boolean = false;

    identityService = inject(IdentityService);
    authService = inject(AuthService);
    router = inject(Router);
    routerAct = inject(ActivatedRoute);
    returnUrl : string = "";

    constructor() { 

    }

    ngOnInit(): void {
      if (this._userService.mEstoyLogueadoLocal()) {
        this.router.navigate(['dashboard']);
      }
      this.returnUrl = this.routerAct.snapshot.queryParams['returnUrl'] || '/dashboard';
      //this._toastService.GeneraAlertaToast(new ToastModel("Cuidado","Estoy en el login"));
    }

  mIntentarAcceder(correo:string, psw:string){
    if(correo != ""){
      let am: UsuarioModel = new UsuarioModel();
      am.mail = correo;
      am.cntrsn = psw;
      this._iniciandoSesion = true;
      this._indentityService.mIntentarAccederAlSitio(am).pipe(
        switchMap(()=>this.authService.mObtenInfoUsuarioLogueado())
      ).subscribe(
        {
          next: value => {
            if(value.exito === true){
              this._userService.mLoguearLocal(value.data);
            }else{
              throw new Error('Error al iniciar sesión.');
            }
          },
          error: err => {this._iniciandoSesion = false; this._intentoFallido = true}, // This will be called if catchError re-throws or doesn't handle the error
          complete: ()=>{
            console.log(this.returnUrl);
            this._routeService.navigateByUrl(this.returnUrl);
          }
        }
      );
    }
      else{

    }
  }

}
