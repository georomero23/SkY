import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ButtonCloseDirective, ButtonDirective, ModalModule } from '@coreui/angular';

@Component({
  selector: 'app-confirm-dialog',
  imports: [ModalModule, ButtonDirective, ButtonCloseDirective],
  templateUrl: './confirm-dialog.component.html'
})
export class ConfirmDialogComponent {
  @Input() visible: boolean = false;
  @Input() title: string = 'Confirmar acción';
  @Input() message: string = '¿Estás seguro?';
  @Input() confirmText: string = 'Sí';
  @Input() cancelText: string = 'No';

  @Output() confirmed = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  

  confirm() {
    this.confirmed.emit();
  }

  cancel() {
    this.cancelled.emit();
  }
}
