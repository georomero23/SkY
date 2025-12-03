import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { ButtonDirective, CardComponent, CardModule, FormDirective, FormLabelDirective, TabDirective, TabPanelComponent, TabsComponent, TabsContentComponent, TabsListComponent, TabsModule } from "@coreui/angular";
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DashboardService } from '@services/InformacionEntrePantallas/dashboard.service';
import { InstalacionModel } from '@models/instalacion-model';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { IconDirective } from '@coreui/icons-angular';
import { iconSubset } from 'src/app/icons/icon-subset';
import { ToastService } from '@services/Front/toast.service';
import { InterfazService } from '@services/API/interfaz.service';
import { CatalogoModel } from '@models/catalogo-model';
import { ClientesService } from '@services/API/clientes-serivce';
import { InfoInstalacionFormComponent } from "src/app/views/shared/info-instalacion-form/info-instalacion-form.component";
import { InfoClienteFormComponent } from 'src/app/views/shared/info-cliente-form/info-cliente-form.component';
import { ClienteModel } from '@models/cliente-model';
import { InfoConexionesFormComponent } from "src/app/views/shared/info-conexiones-form/info-conexiones-form.component";
import { ActivatedRouteSnapshot, CanDeactivate, GuardResult, MaybeAsync, RouterStateSnapshot } from '@angular/router';
import { PendingChangesGuard } from 'src/app/guards/can-deactivate.guard';

@Component({
  selector: 'app-info-instalacion',
  imports: [CardModule, CommonModule, FormDirective, FormsModule, ButtonDirective, TabsListComponent, TabsComponent, IconDirective, TabDirective,
    TabsModule, TabPanelComponent, InfoInstalacionFormComponent, InfoClienteFormComponent, InfoConexionesFormComponent],
  templateUrl: './info-instalacion.component.html',
  styleUrl: './info-instalacion.component.scss'
})
export class InfoInstalacionComponent implements OnInit, CanDeactivate<PendingChangesGuard> {
  #dashboardService = inject(DashboardService);
  #instalacionService = inject(InstalacionesService);
  #clienteService = inject(ClientesService);
  #interfazService  = inject(InterfazService);
  #tostadaService = inject(ToastService);

  iconos = iconSubset;
  _edicionInst: InstalacionModel = new InstalacionModel();

  _infoInstsOriginal: InstalacionModel = new InstalacionModel();
  _infoInsts: InstalacionModel = new InstalacionModel();

  _catalogoTarifas: CatalogoModel[] = [];
  _catalogoEstatusInst: CatalogoModel[] = [];
  _catalogoPlataformas: CatalogoModel[] = [];
  _catalogoGarantias: CatalogoModel[] = [];

  _idInstalacion: number = 0;

  @ViewChild('infoConexionesForm') infoConexionesFormComponent!: InfoConexionesFormComponent;

  constructor(){
    //this._edicionInst = { ...this._inst };
  }

  ngOnInit(): void {
    this._idInstalacion = this.#dashboardService._Instalacion?.idInstalacion!;
    this.#instalacionService.mObtenerInstalacion(this._idInstalacion).subscribe({
      next: inst => {
        if (inst.exito){
          this._infoInsts = inst.data;
          this._infoInstsOriginal = { ...this._infoInsts };
        }else {
          this.#tostadaService.mostrarError("Error al obtener la información de la instalación: " + inst.mensaje);
        } 
      },
      error: err => {
        this.#tostadaService.mostrarError("Error al obtener la información de la instalación.");
      }
    });
  }

  GuardaInfoInstalacion(instApp: InstalacionModel){
    this.#instalacionService.mModificaInstalacion(instApp).subscribe({
      next: res => {
        if(res.exito){
          this.#tostadaService.mostrarExito("Información de la instalación actualizada correctamente.");
          this._infoInsts = res.data;
        }else{
          this.#tostadaService.mostrarError("Error al actualizar la información de la instalación: " + res.mensaje);
        }
      },
      error: err => {
        this.#tostadaService.mostrarError("Error al actualizar la información de la instalación.");
      }
    });
  }

  GuardaInfoCliente(cliente: ClienteModel){
    this.#clienteService.mModificaCliente(cliente).subscribe({
      next: res => {
        if(res.exito){
          this.#tostadaService.mostrarExito("Información del cliente actualizada correctamente.");
          this._infoInsts.idClienteNavigation = res.data;
        }else{
          this.#tostadaService.mostrarError("Error al actualizar la información del cliente: " + res.mensaje);
        }
      },
      error: err => {
        this.#tostadaService.mostrarError("Error al actualizar la información del cliente.");
      }
    });
  }

  canDeactivate(component: PendingChangesGuard, currentRoute: ActivatedRouteSnapshot, currentState: RouterStateSnapshot, nextState: RouterStateSnapshot) {
    if(this.infoConexionesFormComponent == undefined){
      return true;
    }
    return this.infoConexionesFormComponent.canDeactivate();
  }
}
