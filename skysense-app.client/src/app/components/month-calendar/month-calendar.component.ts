import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CardComponent, CardModule } from "@coreui/angular";

@Component({
  selector: 'app-month-calendar',
  imports: [CommonModule, CardModule],
  templateUrl: './month-calendar.component.html',
  styleUrls: ['./month-calendar.component.scss']
})
export class MonthCalendarComponent {
  @Input() year: number = new Date().getFullYear();
  @Input() month: number = new Date().getMonth() + 1; // 1 = Enero
  @Input() selectedDays: number[] = [];
  @Output() selectedDaysChange = new EventEmitter<number[]>();

  diasSemana = ['Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa', 'Do'];

  meses = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
  ];

  get diasEnMes(): number {
    return new Date(this.year, this.month, 0).getDate();
  }

  get primerDiaSemana(): number {
    // 0 = Domingo, 1 = Lunes, ..., 6 = Sábado
    let d = new Date(this.year, this.month - 1, 1).getDay();
    return d === 0 ? 6 : d - 1; // Ajusta para que Lunes sea 0
  }

  get calendario(): (number | null)[][] {
    const semanas: (number | null)[][] = [];
    let dia = 1 - this.primerDiaSemana;
    while (dia <= this.diasEnMes) {
      const semana: (number | null)[] = [];
      for (let i = 0; i < 7; i++) {
        semana.push(dia > 0 && dia <= this.diasEnMes ? dia : null);
        dia++;
      }
      semanas.push(semana);
    }
    return semanas;
  }

  toggleDia(dia: number) {
    if (!dia) return;
    if (this.selectedDays.includes(dia)) {
      this.selectedDays = this.selectedDays.filter(d => d !== dia);
    } else {
      this.selectedDays = [...this.selectedDays, dia];
    }
    this.selectedDaysChange.emit(this.selectedDays);
  }

  esSeleccionado(dia: number): boolean {
    return this.selectedDays.includes(dia);
  }
}
