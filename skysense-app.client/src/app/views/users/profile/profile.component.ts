import { Component, inject } from '@angular/core';
import { ColComponent, TextColorDirective, CardComponent, CardHeaderComponent, CardBodyComponent, FormControlDirective, FormDirective, FormLabelDirective, FormSelectDirective, ButtonDirective } from '@coreui/angular';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { UserService } from '@services/Front/user.service';

@Component({
  selector: 'app-profile',
  imports: [ColComponent, TextColorDirective, CardComponent, CardHeaderComponent, CardBodyComponent, FormControlDirective, ReactiveFormsModule, FormsModule, FormDirective, FormLabelDirective, FormSelectDirective, ButtonDirective],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent {

  #UserService:UserService = inject(UserService);

  _nombre: string;
  _apellidos: string;
  _rol: string;
  _correo: string;

  constructor(){
    const user = this.#UserService.Usuario;
    this._nombre=user?.name??"";
    this._apellidos=user?.name??"";
    this._rol= user?.roles.join(",")??"";
    this._correo=user?.mail??"";

  }
}
