import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PaginationModule } from '@coreui/angular';
import { Paginacion } from '@models/apiRespuestaModel';

@Component({
  selector: 'app-mi-paginador',
  imports: [PaginationModule, CommonModule],
  templateUrl: './mi-paginador.component.html',
  styleUrl: './mi-paginador.component.scss'
})
export class MiPaginadorComponent {

  @Output() onPaginaCambiada = new EventEmitter<number>();
  @Input() set paginado(value: Paginacion<any>) {
    this._paginado = value;
    this._paginasArray = Array.from({ length: value.numeroPaginas }, (_, i) => i + 1);
  }

  _paginado!: Paginacion<any>;
  _paginasArray: number[] = [];

  paginacionCambiada(newPage:number){

    this._paginado.paginaActual = newPage;

    this.onPaginaCambiada.emit(newPage);
  }

  getPaginasParaMostrar(): (number | string)[] {
    const total = this._paginado.numeroPaginas;
    const actual = this._paginado.paginaActual;
    const max = 11; // Máximo de páginas a mostrar (incluyendo '...')
    const paginas: (number | string)[] = [];

    if (total <= max) {
      for (let i = 1; i <= total; i++) paginas.push(i);
      return paginas;
    }

    if (actual <= 6) {
      for (let i = 1; i <= 9; i++) paginas.push(i);
      paginas.push('...');
      paginas.push(total);
    } else if (actual >= total - 5) {
      paginas.push(1);
      paginas.push('...');
      for (let i = total - 8; i <= total; i++) paginas.push(i);
    } else {
      paginas.push(1);
      paginas.push('...');
      for (let i = actual - 3; i <= actual + 3; i++) paginas.push(i);
      paginas.push('...');
      paginas.push(total);
    }
    return paginas;
  }
}
