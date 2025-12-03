import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonDirective, TableDirective } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { iconSubset } from 'src/app/icons/icon-subset';

@Component({
  selector: 'app-registro-editor',
  imports: [CommonModule, FormsModule, ButtonDirective, TableDirective, IconDirective],
  templateUrl: './registro-editor.component.html',
  styleUrls: ['./registro-editor.component.scss']
})
export class RegistroEditorComponent {

  iconos = iconSubset;

  @Input() headers: { key: string, label: string, type?: string, mandatory?: boolean, options?: { value: string, label: string }[] }[] = [];
  @Input() registros: any[] = [];
  @Input() registroEditando: any = null;
  @Input() modoEdicion: boolean = false;
  @Input() idKey: string = 'id';
  @Input() esEstatico: boolean = false;
  @Input() permiteNuevos: boolean = true;

  @Output() guardar = new EventEmitter<any>();
  @Output() cancelar = new EventEmitter<void>();
  @Output() editar = new EventEmitter<any>();
  @Output() eliminar = new EventEmitter<any>();

  nuevoRegistro: any = {};

  iniciarNuevo() {
    this.nuevoRegistro = {};
    this.headers.forEach(h => {
      if (h.type === 'checkbox') {
        this.nuevoRegistro[h.key] = false;
      } else {
        this.nuevoRegistro[h.key] = '';
      }
    });
    this.modoEdicion = true;
  }

  editarRegistro(registro: any) {
    this.registroEditando = { ...registro };
    this.modoEdicion = true;
    console.log(this.headers);
    this.editar.emit(registro);
  }

  guardarRegistro() {
    this.guardar.emit(this.registroEditando || this.nuevoRegistro);
    this.modoEdicion = false;
    this.registroEditando = null;
    this.nuevoRegistro = {};
  }

  cancelarEdicion() {
    this.cancelar.emit();
    this.modoEdicion = false;
    this.registroEditando = null;
    this.nuevoRegistro = {};
  }

  //Se manda solo el id del registro
  eliminarRegistro(registro: any) {
    this.eliminar.emit(registro[this.idKey]);
  }

  get permitirGuardar(): boolean {
    const registro = this.registroActual;
    return this.headers.every(header => {
      if (header.mandatory) { 
        const valor = registro[header.key];
        // Acepta 0 y false, pero no null, undefined o string vacía
        return valor !== null && valor !== undefined && !(typeof valor === 'string' && valor.trim() === '');
      }
      return true;
    });
  } 

  get registroActual() {
    return this.registroEditando || this.nuevoRegistro;
  }

  getOptionLabel(h: any, registro: any): string {
    if (!h.options) return registro[h.key];
    const found = h.options.find((opt: any) => opt.value == registro[h.key]);
    return found ? found.label : registro[h.key];
  }

  getMultiselectLabels(h: any, registro: any): string {
  const values = registro[h.key] || [];
  if (!Array.isArray(values)) return '';
  return values
    .map((v: any) => h.options?.find((opt: any) => opt.value === v)?.label)
    .filter((label: any) => !!label)
    .join(', ');
  }
}