import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CardModule, ColComponent, RowComponent, TableDirective } from '@coreui/angular';
import { MonthCalendarComponent } from "src/app/components/month-calendar/month-calendar.component";
import { InterfazService } from '@services/API/interfaz.service';
import { ToastService } from '@services/Front/toast.service';
import { ButtonsComponent } from '../../buttons/buttons/buttons.component';

@Component({
  selector: 'app-dias-inhabiles',
  imports: [CommonModule, FormsModule, CardModule, MonthCalendarComponent, ColComponent, RowComponent, ButtonsComponent],
  templateUrl: './dias-inhabiles.component.html',
  styleUrls: ['./dias-inhabiles.component.scss']
})
export class DiasInhabilesComponent {

  #interfazService = inject(InterfazService);
  #toastService = inject(ToastService);

  ngOnInit(): void {
    this.cargarDiasFestivos();
  }

  year = new Date().getFullYear();
  yearAvailables: number[] = [];

  month = new Date().getMonth(); // 0 = Enero
  diasFestivos: number[][] = []; // separados por mes y dia



  meses: string[] = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
  ];

  constructor() {
    this.yearAvailables = Array.from({ length: 5 }, (_, i) => this.year + 1 - i);
  }
  
  
  cargarDiasFestivos() {
    this.#interfazService.getDiasFestivos(this.year).subscribe({
      next: (dias) => {
        if(dias.exito){
          this.diasFestivos = Array.from({ length: 12 }, () => []);
          dias.data?.forEach(fechaStr => {
            const fecha = new Date(fechaStr);
            const mes = fecha.getUTCMonth();
            const dia = fecha.getUTCDate();
            this.diasFestivos[mes].push(dia);
          });
        }else{
          this.#toastService.mostrarError(dias.mensaje || 'Error al cargar los días festivos.');
        }
      },
      error: (err) => {
        this.#toastService.mostrarError('Error al cargar los días festivos.');
        console.error(err);
      }
    });
  }

  guardarCambios() {

    const diasFestivosAplanados: Date[] = [];
    this.diasFestivos.forEach((dias, mesIndex) => {
      dias.forEach(dia => {
        diasFestivosAplanados.push(new Date(this.year, mesIndex, dia));
      });
    });

    this.#interfazService.guardarDiasFestivos( this.year, diasFestivosAplanados).subscribe({
      next: (response) => {
        if (response.exito) {
          this.#toastService.mostrarExito('Días festivos guardados correctamente.');
        } else {
          this.#toastService.mostrarError(response.mensaje || 'Error al guardar los días festivos.');
        }
      },
      error: (err) => {
        this.#toastService.mostrarError('Error al guardar los días festivos.');
        console.error(err);
      }
    });

  }

}