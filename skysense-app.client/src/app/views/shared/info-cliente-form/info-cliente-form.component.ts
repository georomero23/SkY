import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonDirective } from '@coreui/angular';
import { ClienteModel } from '@models/cliente-model';

@Component({
  selector: 'app-info-cliente-form',
  imports: [CommonModule, FormsModule, ButtonDirective],
  templateUrl: './info-cliente-form.component.html',
  styleUrl: './info-cliente-form.component.scss'
})
export class InfoClienteFormComponent {
  @Input() infoCliente: ClienteModel = new ClienteModel();

  @Output() guardar = new EventEmitter<ClienteModel>();

  GuardaInfoCliente() {
    this.guardar.emit(this.infoCliente);
  }
}
