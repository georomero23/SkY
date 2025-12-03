import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonDirective } from '@coreui/angular';
import { InstalacionModel } from '@models/instalacion-model';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { InterfazService } from '@services/API/interfaz.service';
import { ToastService } from '@services/Front/toast.service';

@Component({
  selector: 'app-info-instalacion-form',
  imports: [CommonModule, FormsModule, ButtonDirective],
  templateUrl: './info-instalacion-form.component.html',
  styleUrls: ['./info-instalacion-form.component.scss']
})
export class InfoInstalacionFormComponent implements OnInit {

  @Input() infoInsts: InstalacionModel = new InstalacionModel();

  catalogoTarifas: any[] = [];
  catalogoGarantias: any[] = [];
  catalogoEstatusInst: any[] = [];
  catalogoPlataformas: any[] = [];
  catalogoDivisiones: any[] = [];
  catalogoTipoProyecto: any[] = [];

  #instalacionService = inject(InstalacionesService);
  #interfazService  = inject(InterfazService);
  #tostadaService = inject(ToastService);

  @Output() guardar = new EventEmitter<InstalacionModel>();

  GuardaInfoInstalacion() {
    this.guardar.emit(this.infoInsts);
  }

  ngOnInit(): void {
    
    this.#interfazService.obtenerOpcionesCatalogos(1,10,3,2,12,13).subscribe({
      next: opciones => {
        if(opciones.exito){
          opciones.data.forEach(cat => {
            switch(cat.idCatalogo){
              case 1:
                this.catalogoTarifas = cat.opciones;
                break;
              case 2:
                this.catalogoEstatusInst = cat.opciones;
                break;
              case 3:
                this.catalogoGarantias = cat.opciones;
                break;
              case 10:
                this.catalogoPlataformas = cat.opciones;
                break;
              case 12:
                this.catalogoDivisiones = cat.opciones;
                break;
              case 13:
                this.catalogoTipoProyecto = cat.opciones;
                break;
            }
          });
        }else{
          this.#tostadaService.mostrarError("Error al obtener las opciones de catálogo: " + opciones.mensaje);
        }
      },
      error: err => {
        this.#tostadaService.mostrarError("Error al obtener las opciones de catálogo.");
      }
    });
  }
}
