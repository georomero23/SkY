import { Component, inject, NgModule, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ButtonDirective, ButtonGroupComponent, CardBodyComponent, CardComponent, ColComponent, ModalModule, NavComponent, NavItemComponent, NavLinkDirective, RowComponent, TabDirective, TabsComponent, TabsListComponent, TemplateIdDirective, WidgetStatFComponent } from '@coreui/angular';
import { filter, map, Observable, of } from 'rxjs';
import { ClienteDashboard, InstalacionOpcion } from '@models/dashboard-models';
import { ClientesService } from '@services/API/clientes-serivce';
import { DefaultImgDirective } from '../../../directives/DefaultImg.directive';
import { InstalacionDashboard } from '@models/dashboard-models';
import { PotenciaRedondeoPipe } from '../../../pipes/potencia-redondeo.pipe';
import { IconComponent, IconDirective } from '@coreui/icons-angular';
import { TablesComponent } from '../../base/tables/tables.component';
import { AsyncPipe, CommonModule, NgFor } from '@angular/common';
import { iconSubset } from '../../../icons/icon-subset';
import { FormsModule, NgModel } from '@angular/forms';
import { DashboardService } from '@services/InformacionEntrePantallas/dashboard.service';
import { InfoInstalacionFormComponent } from '../../shared/info-instalacion-form/info-instalacion-form.component';
import { InstalacionModel } from '@models/instalacion-model';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { ToastService } from '@services/Front/toast.service';
import { ClienteModel } from '@models/cliente-model';

@Component({
  selector: 'app-dashboard-cliente',
  imports: [CardComponent, CardBodyComponent, RowComponent, ColComponent, ButtonGroupComponent,
    DefaultImgDirective, PotenciaRedondeoPipe, IconComponent, IconDirective, CommonModule,
    TablesComponent, TabDirective, NavComponent, NavLinkDirective, NavItemComponent, TabsListComponent, NgFor, TemplateIdDirective,
    RouterOutlet, RouterLink, AsyncPipe, RouterLinkActive, FormsModule, ModalModule, ButtonDirective, InfoInstalacionFormComponent],
  templateUrl: './dashboard-cliente.component.html',
  styleUrl: './dashboard-cliente.component.scss'
})
export class DashboardClienteComponent implements OnInit, OnDestroy {
  iconos = iconSubset;
  public imagenCliente$: Observable<any>;
  private imagenClienteRuta:string = 'assets/images/c/'
  #cliente = inject(ClientesService);
  #routeService = inject(ActivatedRoute);
  #routerService = inject(Router);
  #dashboardService = inject(DashboardService);
  #instalacionService = inject(InstalacionesService);
  #tostadaService = inject(ToastService);
  private clienteId$;
  instalacionId$;
  _Cliente: ClienteDashboard|null = null;
  _Instalacion: InstalacionDashboard|null = null;
  _valorSeleccionadoInstalacion: InstalacionOpcion | null = null;
  // _cargaDeDatosInstalacionTerminada: boolean = true;
  _modalNewInstVisible = false;

  constructor(){
    this.imagenCliente$ = of(this.imagenClienteRuta + 'no-image.jpeg');
    this.clienteId$ = this.#routeService.params.pipe(map((p)=>p['idCliente']));
    this.instalacionId$ = this.#routeService.children[0].params.pipe(map((p)=>p['idInstalacion']));
  }

  ngOnInit(){
    this.clienteId$.subscribe({
        next: (next)=>{
          this.mActualizaCliente(next);
        },
        error: (error)=>{console.log(error)},
        complete: ()=> {}
      }
    );
  }

  mActualizaCliente(idCliente:number){
    this.#cliente.mObtenerCliente(idCliente).subscribe({
        next: (clienteNext)=>{
          this._Cliente = clienteNext;
          let idInst = +this.#routeService.children[0].snapshot.paramMap.get('idInstalacion')!;
          if(idInst == 0){
            idInst = clienteNext?.arrInstalaciones[0]?.idInstalacion??0;
            this.SelectOnChange({target:{value:clienteNext?.arrInstalaciones[0]??undefined}});
          }
          //console.log(idInst);
          this._valorSeleccionadoInstalacion = clienteNext?.arrInstalaciones.find(ins => ins.idInstalacion === idInst) ?? null;
          // if(this.#routeService.children[0].snapshot.paramMap.get('idInstalacion') == '0' && clienteNext.arrInstalaciones.length > 0)
          //   this.#routerService.navigate([clienteNext.arrInstalaciones[0].idInstalacion],{ relativeTo: this.#routeService});

          
          // if(this._Cliente.arrInstalaciones.length>0){
          //   this.nuevaSeleccionInstalacion(this._Cliente.arrInstalaciones[0].idInstalacion);
          // }else{
            
          // }
        },
        error: (error)=>{console.log(error)},
        complete: ()=> {}
      }
    );
  }

  SelectOnChange(event:any){
     const selectedValue = event.idInstalacion;
     const pathAdicional = this.#routeService.children[0]?.children[0]?.snapshot?.url?.map(u=>u.path)??[];
     this.#routerService.navigate([selectedValue].concat(["Instalaciones"]),{ relativeTo: this.#routeService});
  }

  ngOnDestroy(): void {
    this.#dashboardService._Instalacion = undefined;
  }

  GuardaInfoInstalacion($event: InstalacionModel) {
    $event.idCliente = this._Cliente?.cClienteInfo?.idCliente??0;
    $event.idClienteNavigation = new ClienteModel();
    this.#instalacionService.mNuevaInstalacion($event).subscribe({
      next: (response) => {
        if(response.exito){
          this.#tostadaService.mostrarExito('Instalación guardada con éxito.');
          this.mActualizaCliente($event.idCliente);
          this._modalNewInstVisible = false;
          
        }else{
          this.#tostadaService.mostrarError('Error al guardar instalación: ' + response.mensaje);
        }
      },
      error: (error) => {
          this.#tostadaService.mostrarError('Error al guardar instalación.');
      }
    });
  }
  compareByOptionId(option1: InstalacionOpcion, option2: InstalacionOpcion): boolean {
    return option1 && option2 && option1.idInstalacion === option2.idInstalacion;
  }
}
