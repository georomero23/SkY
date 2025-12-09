import { Component, inject, OnDestroy, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import {
  CardBodyComponent,
  CardComponent,
  ColComponent,
  ModalModule,
  NavComponent,
  NavItemComponent,
  NavLinkDirective,
  RowComponent,
  TabDirective,
  TabsListComponent,
  TemplateIdDirective
} from '@coreui/angular';

import { map, Observable, of } from 'rxjs';
import { ClienteDashboard, InstalacionOpcion } from '@models/dashboard-models';
import { ClientesService } from '@services/API/clientes-serivce';
import { DefaultImgDirective } from '../../../directives/DefaultImg.directive';
import { PotenciaRedondeoPipe } from '../../../pipes/potencia-redondeo.pipe';
import { IconComponent, IconDirective } from '@coreui/icons-angular';
import { TablesComponent } from '../../base/tables/tables.component';
import { AsyncPipe, CommonModule, NgFor } from '@angular/common';
import { iconSubset } from '../../../icons/icon-subset';
import { FormsModule } from '@angular/forms';
import { DashboardService } from '@services/InformacionEntrePantallas/dashboard.service';
import { InfoInstalacionFormComponent } from '../../shared/info-instalacion-form/info-instalacion-form.component';
import { InstalacionModel } from '@models/instalacion-model';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { ToastService } from '@services/Front/toast.service';
import { ClienteModel } from '@models/cliente-model';

@Component({
  selector: 'app-dashboard-cliente',
  standalone: true,
  imports: [
    CardComponent, CardBodyComponent, RowComponent, ColComponent,
    DefaultImgDirective, PotenciaRedondeoPipe, IconComponent,
    IconDirective, CommonModule, TablesComponent, TabDirective,
    NavComponent, NavLinkDirective, NavItemComponent, TabsListComponent,
    NgFor, TemplateIdDirective, AsyncPipe, FormsModule, ModalModule,
    InfoInstalacionFormComponent,

    RouterLink,
    RouterLinkActive,
    RouterOutlet
  ],
  templateUrl: './dashboard-cliente.component.html',
  styleUrls: ['./dashboard-cliente.component.scss']
})
export class DashboardClienteComponent implements OnInit, OnDestroy {

  iconos = iconSubset;
  imagenCliente$: Observable<any>;
  private imagenClienteRuta = 'assets/images/c/';

  #cliente = inject(ClientesService);
  #routeService = inject(ActivatedRoute);
  #routerService = inject(Router);
  #dashboardService = inject(DashboardService);
  #instalacionService = inject(InstalacionesService);
  #tostadaService = inject(ToastService);
  #cdr = inject(ChangeDetectorRef);

  clienteId$;
  instalacionId$;
  _Cliente: ClienteDashboard | null = null;

  _valorSeleccionadoInstalacionId: number | null = null;
  _valorSeleccionadoInstalacion: InstalacionOpcion | null = null;

  _modalNewInstVisible = false;

  constructor() {
    this.imagenCliente$ = of(this.imagenClienteRuta + 'no-image.jpeg');

    this.clienteId$ = this.#routeService.params.pipe(
      map((p) => p['idCliente'])
    );

    this.instalacionId$ = this.#routeService.firstChild?.params.pipe(
      map(p => Number(p['idInstalacion']))
    );
  }

  ngOnInit(): void {
    this.clienteId$.subscribe(id => this.mActualizaCliente(id));
  }

  private tryNavigate(instId: number, seccion: string) {
    setTimeout(() => {
      this.#routerService.navigate([instId, seccion], { relativeTo: this.#routeService })
        .catch(() => {
          const idCliente = this.#routeService.snapshot.paramMap.get('idCliente');
          if (idCliente) {
            this.#routerService.navigate(['/clientes', idCliente, instId, seccion]);
          }
        });
    }, 0);
  }

  mActualizaCliente(idCliente: number) {
    this.#cliente.mObtenerCliente(idCliente).subscribe({
      next: (clienteData) => {
        this._Cliente = clienteData;
        const primera = clienteData.arrInstalaciones?.[0] ?? null;

        const instUrl = Number(this.#routeService.firstChild?.snapshot.paramMap.get('idInstalacion')) || 0;

        if (!instUrl || !clienteData.arrInstalaciones.some(i => i.idInstalacion === instUrl)) {
          if (primera) {
            this._valorSeleccionadoInstalacion = primera;
            this._valorSeleccionadoInstalacionId = primera.idInstalacion;

            this.#cdr.detectChanges();
            this.tryNavigate(primera.idInstalacion, 'Instalaciones');
          }
        } else {
          this._valorSeleccionadoInstalacion =
            clienteData.arrInstalaciones.find(i => i.idInstalacion === instUrl) ?? null;

          this._valorSeleccionadoInstalacionId =
            this._valorSeleccionadoInstalacion?.idInstalacion ?? null;

          this.#cdr.detectChanges();
        }
      }
    });
  }

  SelectOnChangeById(newId: number) {
    if (!newId) return;

    this._valorSeleccionadoInstalacionId = newId;
    this._valorSeleccionadoInstalacion =
      this._Cliente?.arrInstalaciones.find(i => i.idInstalacion === newId) ?? null;

    this.#cdr.detectChanges();

    const seccionActual =
      this.#routeService.firstChild?.firstChild?.snapshot?.url?.[0]?.path ||
      this.#routeService.firstChild?.snapshot?.url?.[0]?.path ||
      'Instalaciones';

    this.tryNavigate(newId, seccionActual);
  }

  SelectOnChange(event: any) {
    const id = event?.idInstalacion ?? Number(event) ?? null;
    if (id) this.SelectOnChangeById(id);
  }

  GuardaInfoInstalacion(event: InstalacionModel) {
    event.idCliente = this._Cliente?.cClienteInfo?.idCliente ?? 0;
    event.idClienteNavigation = new ClienteModel();

    this.#instalacionService.mNuevaInstalacion(event).subscribe({
      next: response => {
        if (response.exito) {
          this.#tostadaService.mostrarExito('Instalación guardada con éxito.');
          this.mActualizaCliente(event.idCliente);
          this._modalNewInstVisible = false;
        } else {
          this.#tostadaService.mostrarError('Error: ' + response.mensaje);
        }
      },
      error: () => this.#tostadaService.mostrarError('Error en el guardado.')
    });
  }

  ngOnDestroy(): void {
    this.#dashboardService._Instalacion = undefined;
  }
}
