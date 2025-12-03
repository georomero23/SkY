import { Component, forwardRef, Input } from '@angular/core';
import { ToastBodyComponent, ToastCloseDirective, ToastComponent, ToastHeaderComponent } from '@coreui/angular';

@Component({
  selector: 'app-tostada',
  templateUrl: './tostada.component.html',
  styleUrl: './tostada.component.scss',
  providers: [{ provide: ToastComponent, useExisting: forwardRef(() => TostadaComponent) }],
  standalone: true,
  imports: [ToastHeaderComponent, ToastBodyComponent, ToastCloseDirective]
})
export class TostadaComponent extends ToastComponent  {
  constructor() {
    super();
  }

  @Input() closeButton = true;
  @Input() mensaje = '';
  @Input() title = '';
}
