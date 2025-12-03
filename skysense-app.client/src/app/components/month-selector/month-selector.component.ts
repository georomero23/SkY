import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { ButtonDirective } from '@coreui/angular';
import { iconSubset, IconSubset } from 'src/app/icons/icon-subset';
import { IconDirective } from "@coreui/icons-angular";
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-month-selector',
  imports: [ButtonDirective, IconDirective, FormsModule, CommonModule],
  templateUrl: './month-selector.component.html',
  styleUrls: ['./month-selector.component.scss']
})
export class MonthSelectorComponent implements OnInit {
  @Input() year: number = new Date().getFullYear();
  @Input() month: number = new Date().getMonth() + 1; // 1 = Enero
  @Output() monthChange = new EventEmitter<number>();
  @Output() yearChange = new EventEmitter<number>();
  @Output() dateChange = new EventEmitter<{year: number, month: number}>();

  // Emite un evento cada vez que cambian el mes o el año
  ngOnChanges() {
    this.dateChange.emit({year: this.year, month: this.month});
  }

  iconos = iconSubset;

  meses = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
  ];

  anios: number[] = [];

  ngOnInit() {
    const currentYear = new Date().getFullYear();
    // Por ejemplo, muestra los últimos 5 años
    this.anios = Array.from({length: 5}, (_, i) => currentYear - i);
  }

  avanzar() {
    if (this.month < 12) {
      this.month++;
      this.monthChange.emit(this.month);
    } else {
      this.month = 1;
      this.year++;
      this.monthChange.emit(this.month);
      this.yearChange.emit(this.year);
    }
  }

  retroceder() {
    if (this.month > 1) {
      this.month--;
      this.monthChange.emit(this.month);
    } else {
      this.month = 12;
      this.year--;
      this.monthChange.emit(this.month);
      this.yearChange.emit(this.year);
    }
  }

  onMonthChange(newMonth: number) {
    this.month = newMonth;
    this.monthChange.emit(this.month);
  }

  onYearChange(newYear: number) {
    this.year = newYear;
    this.yearChange.emit(this.year);
  }
}