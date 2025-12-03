import { Component, inject, OnInit } from '@angular/core';
import { InterfazService } from '@services/API/interfaz.service';
import { ToastService } from '@services/Front/toast.service';
import { ButtonDirective, CardComponent, CardModule, ColComponent, ColDirective, PlaceholderAnimationDirective, PlaceholderDirective, RowComponent } from "@coreui/angular";
import { FormsModule } from '@angular/forms';
import { CatalogoModel, ZonaTarifaModel } from '@models/catalogo-model';
import { MonthSelectorComponent } from "src/app/components/month-selector/month-selector.component";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-zonas-tarifas',
  imports: [CardModule, FormsModule, CommonModule, MonthSelectorComponent,
    PlaceholderAnimationDirective, PlaceholderDirective, ColDirective, ButtonDirective, RowComponent, ColComponent],
  templateUrl: './zonas-tarifas.component.html',
  styleUrl: './zonas-tarifas.component.scss'
})
export class ZonasTarifasComponent implements OnInit {

  #interfazService = inject(InterfazService);
  #tostadaService  = inject(ToastService);

  catalogoZonas: CatalogoModel[] = [];
  catalogoTarifas: CatalogoModel[] = [];

  anoSeleccionado: number = new Date().getFullYear();
  mesSeleccionado: number = new Date().getMonth() + 1;

  zonaTarifa: ZonaTarifaModel[] = [];
  editando: boolean = false;
  zonaTarifaEditando: ZonaTarifaModel[] = [];
  idZona: number = 0;

  esperando: boolean = false;

  ngOnInit(): void {

    this.esperando = true;

    this.#interfazService.obtenerOpcionesCatalogos(1,12).subscribe({
      next: (data) => {
        if(data.exito && data.data.length === 2) {
          this.catalogoZonas = data.data[1].opciones;
          this.idZona = this.catalogoZonas.length > 0 ? this.catalogoZonas[0].idOpcion : 0;
          this.cargarZonasTarifas();
          this.catalogoTarifas = data.data[0].opciones.filter(t => t.nombreOpcion.length > 0); // Excluir tarifa con id 3
        } else {
          this.#tostadaService.mostrarError('Error al obtener opciones de catálogo: ' + data.mensaje);
        }
      },
      error: () => {
        this.#tostadaService.mostrarError('Error al obtener opciones de catálogo');
        this.esperando = false;
      },
      complete: () => { this.esperando = false; }
    }); 

    // this.cargarZonasTarifas();
  }

  cargarZonasTarifas() {
    this.esperando = true;

    this.#interfazService.obtenerZonasTarifas(this.anoSeleccionado, this.mesSeleccionado, this.idZona).subscribe({
      next: (data) => {
        if (data.exito) {
          this.zonaTarifa = data.data;
          this.zonaTarifaEditando=JSON.parse(JSON.stringify(this.zonaTarifa));
        } else {
          this.#tostadaService.mostrarError('Error al obtener zonas y tarifas: ' + data.mensaje);
        }
      },
      error: () => {
        this.#tostadaService.mostrarError('Error al obtener zonas y tarifas');
        this.esperando = false;
      },
      complete: () => { this.esperando = false; }
    });
  }

  getZonaTarifa(zonaId: number, tarifaId: number):ZonaTarifaModel | undefined {
    const zona = this.zonaTarifaEditando.find(z => z.idDivision == zonaId && z.idTarifa == tarifaId);
    return zona;
  }

  guardarCambios() {
    this.#interfazService.guardarZonaTarifa(this.anoSeleccionado, this.mesSeleccionado, this.idZona, this.zonaTarifaEditando).subscribe({
      next: () => {
        this.#tostadaService.mostrarExito('Cambios guardados correctamente');
        this.cancelarEdicion(true);
      },
      error: () => {
        this.#tostadaService.mostrarError('Error al guardar cambios');
      }
    });
  }
  
  cancelarEdicion(exito: boolean = false) {
    this.editando = false;
    if(!exito)
      this.zonaTarifaEditando = JSON.parse(JSON.stringify(this.zonaTarifa));
    else
      this.cargarZonasTarifas();
  }

  editarRegistros() {
    this.zonaTarifaEditando = JSON.parse(JSON.stringify(this.zonaTarifa));
    this.editando = true;
  }
  
  setZonaTarifa(zonaId: number, tarifaId: number, event: Event, elemento: string) {
    const input = event.target as HTMLInputElement;
    const valor = Number(input.value);

    const zonTarif = this.getZonaTarifa(zonaId, tarifaId)??new ZonaTarifaModel();

    if (zonTarif.idTarifa == 0) {
      this.zonaTarifaEditando.push(zonTarif);

      zonTarif.idDivision = zonaId;
      zonTarif.idTarifa = tarifaId;
      zonTarif.anno = this.anoSeleccionado;
      zonTarif.mes = this.mesSeleccionado;

    }

    
    switch(elemento) {
      case 'valorCapacidad':
        zonTarif.valorCapacidad = valor;
        break;
      case 'valorEnergiaBase':
        zonTarif.valorEnergiaBase = valor;
        break;
      case 'valorEnergiaIntermedia':
        zonTarif.valorEnergiaIntermedia = valor;
        break;
      case 'valorEnergiaPunta':
        zonTarif.valorEnergiaPunta = valor;
        break;
      case 'valorEnergiaSemipunta':
        zonTarif.valorEnergiaSemipunta = valor;
        break;
      case 'valorDistribucion':
        zonTarif.valorDistribucion = valor;
        break;
      case 'valorOpCenace':
        zonTarif.valorOpCenace = valor;
        break;
      case 'valorOpSsb':
        zonTarif.valorOpSsb = valor;
        break;
      case 'valorServiciosNoMem':
        zonTarif.valorServiciosNoMem = valor;
        break;
      case 'valorTransmision':
        zonTarif.valorTransmision = valor;
        break;
    }
  }

    
  cambiarVista(arg0: number) {
    this.idZona = arg0 ;
    this.cargarZonasTarifas();
  }
}
