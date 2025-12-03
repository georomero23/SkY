import { CommonModule } from '@angular/common';
import { Component, Input, input } from '@angular/core';
import { ColComponent, ModalModule, RowComponent, SpinnerComponent } from '@coreui/angular';
import { InversorModel } from '@models/inversor-model';
import { PanelModel } from '@models/panel-model';

@Component({
  selector: 'app-modal-tabla',
  imports: [ModalModule, RowComponent, ColComponent, SpinnerComponent, CommonModule],
  templateUrl: './modal-tabla.component.html',
  styleUrl: './modal-tabla.component.scss'
})
export class ModalTablaComponent {
    @Input() _datosTabla: (PanelModel | InversorModel)[] = [];
    @Input() tipoTabla!: 'panel' | 'inversor';

    _guardandoDatos: boolean = false;

}
