import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { CardModule } from '@coreui/angular';
import { BateriaTabla } from '@models/baterias-models';
import { BateriasService } from '@services/API/baterias.service';
import { ToastService } from '@services/Front/toast.service';
import { repeat } from 'rxjs';

@Component({
  selector: 'app-baterias-consulta',
  imports: [CardModule, CommonModule],
  templateUrl: './baterias-consulta.component.html',
  styleUrl: './baterias-consulta.component.scss'
})
export class BateriasConsultaComponent implements OnInit, OnDestroy {
  #bateriasService = inject(BateriasService);
  #tostadaService = inject(ToastService);

  bateriasTabla:BateriaTabla = new BateriaTabla();

  suscripcion$:any;

  ngOnInit(){

    //Se actualiza la tabla cada 5 segundos
    this.suscripcion$ = this.#bateriasService.ObtenTablaBaterias().pipe(repeat({ delay: 5000 })).subscribe({
      next: (data) => {
        if(data && data.exito){
          this.bateriasTabla = data.data;
        }else{
          this.#tostadaService.mostrarError("Error al obtener la tabla de baterías: " + data.mensaje);
        }
      },
      error: (error) => {
          this.#tostadaService.mostrarError("Error al obtener la tabla de baterías: " + error.message);
      }
    })
  }
  
  ngOnDestroy(): void {
    this.suscripcion$.unsubscribe();
  }
}
