import { Component, inject } from '@angular/core';
import { IconDirective } from '@coreui/icons-angular';
import { ContainerComponent, RowComponent, ColComponent, TextColorDirective, CardComponent, CardBodyComponent, FormDirective, InputGroupComponent, InputGroupTextDirective, FormControlDirective, ButtonDirective, FormLabelDirective, FormFeedbackComponent } from '@coreui/angular';
import { IdentityService } from '@services/API/identity.service';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ToastService } from '@services/Front/toast.service';
import { Router } from '@angular/router';
import { ToastModel } from '@models/toast-model';
import { UsuarioModel } from '@models/usuarioModel';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss'],
    imports: [ContainerComponent, RowComponent, ColComponent, TextColorDirective, CardComponent, CardBodyComponent, FormDirective, InputGroupComponent, InputGroupTextDirective, IconDirective, FormControlDirective, ButtonDirective,
      ReactiveFormsModule, FormsModule, FormLabelDirective, FormFeedbackComponent, CommonModule]
})
export class RegisterComponent {

  _authService = inject(IdentityService);
    formBuilder = inject(FormBuilder);
    _toastService = inject(ToastService);
    _routerService = inject(Router);
    customStylesValidated = false;
    registroForm!: FormGroup;

  constructor() {
    this.registroForm = this.formBuilder.group(
          {
            correo: ['', [Validators.required, Validators.email]],
            contrasena: ['', [Validators.required]],
            contrasena2: ['', [Validators.required]]
          }, {
          validator: MustMatch('contrasena', 'contrasena2') // Apply the custom validator here
          }
        );
   }


  onSubmit1() {
    this.customStylesValidated = true;
    
    if(this.registroForm.valid){
      this._authService.mRegistrarUsuario(this.registroForm.controls["correo"].value, this.registroForm.controls["contrasena"].value).subscribe({
        next: v=>{
          this._toastService.GeneraAlertaToast(new ToastModel("Éxito","Se ha registrado al usuario exitosamente. Verifique su correo.",10))
          this._routerService.navigate(["/login"]);
        },
        error: err=> { if(err.status == 400){
          var mensajeError = '';
          var bIrAlLogin = false;
          switch (err.errors) {
            case 'DuplicateUserName':
              mensajeError =
                'El usuario ya existe. Favor de ingresar con ese correo.';
              bIrAlLogin = true;
              break;
            case 'PasswordTooShort':
              mensajeError =
                'La contraseña debe contener al menos 6 caracteres.';
              break;
            case 'PasswordRequiresNonAlphanumeric':
              mensajeError =
                'La contraseña debe contener al menos 1 caracter especial.';
              break;
            case 'PasswordRequiresLower':
              mensajeError =
                'La contraseña debe contener al menos 1 letra minúscula.';
              break;
            case 'PasswordRequiresUpper':
              mensajeError =
                'La contraseña debe contener al menos 1 letra mayúscula.';
              break;
              case 'InvalidEmail':
              mensajeError =
                'El correo electrónico introducido no es válido.';
              break;
              case 'InvalidUserName':
              mensajeError =
                'El nombre de usuario no es válido.';
              break;
              default:
              mensajeError =
                'Ocurrió un error inesperado al registrar al usuario.';
              break;
          }

          this._toastService.GeneraAlertaToast(
            new ToastModel('Error', mensajeError)
          );

          if(bIrAlLogin){
            this._routerService.navigate(['/login']);
          }
        }else{
          this._toastService.GeneraAlertaToast(
            new ToastModel('Error', 'Ocurrió un error inesperado al registrar al usuario.')
          );
        }
        
      },
        complete: () => {}
      });
    }
  }
}

export function MustMatch(controlName: string, matchingControlName: string): ValidatorFn {
      return (formGroup: AbstractControl): { [key: string]: any } | null => {
        const control = formGroup.get(controlName);
        const matchingControl = formGroup.get(matchingControlName);

        if (!control || !matchingControl) {
          return null; // Return null if controls are not found
        }

        // Set error on the matchingControl if values don't match
        if (matchingControl.errors && !matchingControl.errors['mustMatch']) {
          return null; // Return if another validator has already found an error
        }

        if (control.value !== matchingControl.value) {
          matchingControl.setErrors({ mustMatch: true });
        } else {
          matchingControl.setErrors(null); // Clear the error if values match
        }

        return null; // The error is set directly on matchingControl, so return null from the group validator
      };
    }