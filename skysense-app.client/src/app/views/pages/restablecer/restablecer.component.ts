import { Component, inject } from '@angular/core';
import { IconDirective } from '@coreui/icons-angular';
import { ContainerComponent, RowComponent, ColComponent, TextColorDirective, CardComponent, CardBodyComponent, FormDirective, InputGroupComponent, InputGroupTextDirective, FormControlDirective, ButtonDirective, FormFeedbackComponent, FormLabelDirective } from '@coreui/angular';
import { IdentityService } from '@services/API/identity.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastService } from '@services/Front/toast.service';
import { ToastModel } from '@models/toast-model';
import { Router } from '@angular/router';

@Component({
    selector: 'app-restablecer',
    templateUrl: './restablecer.component.html',
    styleUrls: ['./restablecer.component.scss'],
    imports: [ContainerComponent, RowComponent, ColComponent, TextColorDirective, CardComponent, CardBodyComponent, FormDirective, InputGroupComponent, InputGroupTextDirective, IconDirective, FormControlDirective, ButtonDirective,
      ReactiveFormsModule, FormsModule, FormDirective, FormLabelDirective, FormFeedbackComponent
    ]
})
export class RestablecerComponent {
  _authService = inject(IdentityService);
  formBuilder = inject(FormBuilder);
  _toastService = inject(ToastService);
  _routerService = inject(Router);
  inputCorreo = <HTMLInputElement> document.getElementById("")
  customStylesValidated = false;
  restablecerForm!: FormGroup;

  constructor() {

    this.restablecerForm = this.formBuilder.group(
      {
        correo: ['', [Validators.required]]
      }
    );

   }

  onSubmit1() {
    this.customStylesValidated = true;
    
    if(this.restablecerForm.valid){
      this._authService.mOlvideContrasena(this.restablecerForm.controls["correo"].value).subscribe({
        next: v=>{
          this._toastService.GeneraAlertaToast(new ToastModel("Éxito","Se ha mandado un mensaje a tu correo con las instrucciones para restablecer tu contraseña.",10))
          this._routerService.navigate(["/login"]);
        },
        error: err=> { console.log("err")},
        complete: () => {}
      });
    }
  }
}
