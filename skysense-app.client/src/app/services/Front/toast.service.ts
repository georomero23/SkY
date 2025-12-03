import { Injectable } from '@angular/core';
import { NivelAlerta, ToastModel } from '@models/toast-model';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  mostrarError(mensaje: string) {
    this.GeneraAlertaToast(new ToastModel('Error', mensaje, 5, NivelAlerta.Peligro));
  }

  public toastrSubject$ = new Subject();
  constructor() {
   }

   public GeneraAlertaToast(toast:ToastModel){
    this.toastrSubject$.next(toast);
   }
  mostrarExito(mensaje: string) {
    this.GeneraAlertaToast(new ToastModel('Éxito', mensaje, 5, NivelAlerta.Exito));
  }

   public GeneraAlertaToastDirecta(Encabezado: string|undefined,Texto: string, alerta:NivelAlerta|undefined = undefined){
    this.GeneraAlertaToast(new ToastModel(Encabezado, Texto, 5 , alerta));
   }

  //  public GeneraAlertaToastDirecta(Encabezado: string|undefined,Texto: string, alerta: NivelAlerta){
  //   this.GeneraAlertaToast(new ToastModel(Encabezado, Texto,5,alerta));
  //  }
}
