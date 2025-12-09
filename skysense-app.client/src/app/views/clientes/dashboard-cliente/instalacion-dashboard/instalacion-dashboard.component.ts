import { CommonModule, NgFor, NgIf } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonDirective, ButtonGroupComponent, CardBodyComponent, CardComponent, ColComponent, FormCheckComponent, FormCheckInputDirective, FormCheckLabelDirective, FormDirective, FormLabelDirective, ModalBodyComponent, ModalComponent, ModalFooterComponent, ModalHeaderComponent, ModalToggleDirective, RowComponent, SpinnerComponent, TabDirective, TabsComponent, TabsListComponent, TemplateIdDirective, FormControlDirective, WidgetStatFComponent, ButtonCloseDirective } from '@coreui/angular';
import { IconDirective, IconSetService } from '@coreui/icons-angular';
import { iconSubset } from '../../../../icons/icon-subset';
import { map, Observable, Subscription } from 'rxjs';
import { DefaultImgDirective } from '../../../../../app/directives/DefaultImg.directive';
import { TablesComponent } from '../../../../../app/views/base/tables/tables.component';
import { InstalacionDashboard } from '@models/dashboard-models';
import { PotenciaRedondeoPipe } from '../../../../pipes/potencia-redondeo.pipe';
import { ClientesService } from '@services/API/clientes-serivce';
import { DashboardService } from '@services/InformacionEntrePantallas/dashboard.service';
import { PanelModel } from '@models/panel-model';
import { read, WorkBook, WorkSheet,utils } from 'xlsx';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { ToastService } from '@services/Front/toast.service';
import { FormsModule } from '@angular/forms';
import { c } from '@angular/core/navigation_types.d-u4EOrrdZ';
import { InversorModel } from '@models/inversor-model';
import { InversoresService } from '@services/API/inversores-service';
import { RegistroEditorComponent } from "src/app/components/registro-editor/registro-editor.component";
import { NivelAlerta } from '@models/toast-model';
import { FileUploadComponent } from "src/app/components/cargaArchivos/file-upload.component";

@Component({
  selector: 'app-instalacion-dashboard',
  imports: [CardComponent, CardBodyComponent, RowComponent, ColComponent, ButtonGroupComponent, ButtonCloseDirective,
    TabsComponent, PotenciaRedondeoPipe, IconDirective, ModalComponent, ModalHeaderComponent, ModalFooterComponent,
    TablesComponent, WidgetStatFComponent, TabDirective, FormControlDirective, TabsListComponent, NgFor, TemplateIdDirective,
    ModalToggleDirective, ModalBodyComponent, SpinnerComponent, CommonModule, ButtonDirective, FormCheckLabelDirective,
    FormCheckComponent, FormCheckInputDirective, FormsModule, FormLabelDirective, FormDirective, RegistroEditorComponent, FileUploadComponent],
  providers: [IconSetService],
  templateUrl: './instalacion-dashboard.component.html',
  styleUrl: './instalacion-dashboard.component.scss'
})
export class InstalacionDashboardComponent implements OnInit, OnDestroy {
  porcentajeEficacia: number | null = null;
  generacionReal: number | null = null;
  generacionGarantizada: number | null = null;

  _cargaDeDatosInstalacionTerminada: boolean = true;
  #cliente = inject(ClientesService);
  #instalaciones = inject(InstalacionesService);
  #toastr = inject(ToastService);
  _Instalacion: InstalacionDashboard|null = null; 
  #routeService = inject(ActivatedRoute);
  #routerService = inject(Router);
  #DashboardService = inject(DashboardService);
  #InversoresService = inject(InversoresService);
  instalacionId$: Observable<number>;
  instalacionId$S: Subscription|undefined;
iconos = iconSubset
_guardandoDatos: boolean = false;

_panelesImportados: PanelModel[] = [];
_paneles: PanelModel[] = [];
_cantidadPaneles: number = 0;
_potenciaPaneles: number = 0;

_inversores: InversorModel[] = [];
_inversoresImportados: InversorModel[] = [];
  _inversorInsertar: InversorModel | null = new InversorModel();

  estatusColor: string = 'light';
  estatusTexto: string = '';

  constructor(){
    this.instalacionId$ = this.#routeService.parent!.params.pipe(map((p)=>p['idInstalacion']));
    if(this.#DashboardService._Instalacion === null){
      this.#routerService.navigate(['.'], { relativeTo: this.#routeService.parent });
    }
  }

  ngOnInit(){

    this.instalacionId$S = this.instalacionId$.subscribe({
      next: i=> {
        this.nuevaSeleccionInstalacion(i);
      },
      error: er=> console.log(er),
      complete: ()=>{}
    });
  }


  nuevaSeleccionInstalacion(idInstalacion: number) {
    this._cargaDeDatosInstalacionTerminada = false;

    this.#cliente
      .mObtenerInstalacionDelCliente(
        this.#routeService.parent!.parent!.snapshot.params['idCliente'],
        idInstalacion
      )
      .subscribe({
        next: (next) => {
          if (next != null) {
            next.dtFechaInicioOperaciones = new Date(
              next.dtFechaInicioOperaciones
            );

            const previo = this.#instalaciones._UltimaSeleccion;

            this.generacionReal = previo?.generacionReal ?? null;
            this.generacionGarantizada = previo?.generacionGarantizada ?? null;
            this.porcentajeEficacia = previo?.porcentaje ?? null;


            this.#DashboardService._Instalacion = next;
            this._Instalacion = next;

            this.ObtenPaneles();
            this.ObtenerInversores();

            this.calcularPorcentaje();
            this.calcularEstatus();
          }
        },
        error: (error) => {
          console.log(error);
          this._cargaDeDatosInstalacionTerminada = true;
          this.#DashboardService._Instalacion = undefined;
        },
        complete: () => {
          this._cargaDeDatosInstalacionTerminada = true;
        },
      });
  }




  ObtenerInversores(){
    this.#InversoresService.mObtenerInversores(this.#DashboardService._Instalacion?.idInstalacion).subscribe({
      next: (inversores) => {
        this._inversores = inversores;
      },
      error: (error) => {
        console.log(error);
      },
      complete: () => {}
    });
  }

  ngOnDestroy(): void {
    this.instalacionId$S?.unsubscribe();
  }

  onFileSelectedPanel(file: any) {
    //const file = event.target.files[0];
    //console.log(file);
    if (file) {
      const reader = new FileReader();
      this._panelesImportados.length = 0;
      reader.onload = (e: any) => {
        const arrayBuffer: ArrayBuffer = e.target.result;
        const wb: WorkBook = read(arrayBuffer, { type: 'array' });

        // Get the first sheet's name and data
        const wsname: string = wb.SheetNames[0];
        const ws: WorkSheet = wb.Sheets[wsname];

        // Convert sheet data to JSON
        const datos: unknown[] = utils.sheet_to_json(ws, { header: 1 }).slice(1);

        datos.forEach(d => {
          const dato = d as unknown[];
          var nuevoValor = new PanelModel();
          nuevoValor.numeroSerie = (dato[1] as string).toString();
          nuevoValor.marca = dato[2] as string;
          nuevoValor.modelo = dato[3] as string;
          nuevoValor.potencia = dato[4] as number;
          nuevoValor.degradacionAnual = parseFloat((dato[5] as number).toString().replace(',', '.'));
          nuevoValor.proveedorSuministrador = dato[6] as string;
          nuevoValor.proveedorSuministradorRfc = dato[7] as string;
          nuevoValor.proveedorIntermediario = dato[8] as string;
          nuevoValor.proveedorIntermediarioRfc = dato[9] as string;
          nuevoValor.esInicial = (dato[10] as string) == "V";
          nuevoValor.danado = (dato[11] as string) == "V";
          nuevoValor.numeroSerieReemplazo = (dato[12] as string) || null;

          this._panelesImportados.push(nuevoValor);
        });
      };
      reader.readAsArrayBuffer(file); // Use ArrayBuffer instead of binary string
    }
  } 

    onFileSelectedInversor(file: any) {
    //const file = event.target.files[0];
    //console.log(file);
    if (file) {
      const reader = new FileReader();
      this._panelesImportados.length = 0;
      reader.onload = (e: any) => {
        const arrayBuffer: ArrayBuffer = e.target.result;
        const wb: WorkBook = read(arrayBuffer, { type: 'array' });

        // Get the first sheet's name and data
        const wsname: string = wb.SheetNames[0];
        const ws: WorkSheet = wb.Sheets[wsname];

        // Convert sheet data to JSON
        const datos: unknown[] = utils.sheet_to_json(ws, { header: 1 }).slice(1);

        datos.forEach(d => {
          const dato = d as unknown[];
          var nuevoValor = new InversorModel();
          nuevoValor.numeroSerie = (dato[1] as string).toString();
          nuevoValor.marca = dato[2] as string;
          nuevoValor.modelo = dato[3] as string;
          nuevoValor.potencia = dato[4] as number;
          nuevoValor.degradacionAnual = parseFloat((dato[5] as number).toString().replace(',', '.'));
          nuevoValor.proveedorSuministrador = dato[6] as string;
          nuevoValor.proveedorSuministradorRfc = dato[7] as string;
          nuevoValor.proveedorIntermediario = dato[8] as string;
          nuevoValor.proveedorIntermediarioRfc = dato[9] as string;
          nuevoValor.esInicial = (dato[10] as string) == "V";
          nuevoValor.danado = (dato[11] as string) == "V";
          nuevoValor.numeroSerieReemplazo = (dato[12] as string) || null;

          this._inversoresImportados.push(nuevoValor);
        });
      };
      reader.readAsArrayBuffer(file); // Use ArrayBuffer instead of binary string
    }
  } 

  GuardarImportacionArchivoPaneles(){
this._guardandoDatos = true;

    this.#instalaciones.mInsertaPaneles(this._Instalacion?.idInstalacion!,this._panelesImportados).subscribe({
      next: () => {
        this.#toastr.GeneraAlertaToastDirecta("Paneles", "Se han insertado correctamente los paneles.");
        this._panelesImportados = [];
        this.ObtenPaneles();
      },
      error: (err)=>{ this.#toastr.GeneraAlertaToastDirecta("Error", "Ocurrió un error al insertar los paneles.");
        this._guardandoDatos = false;
       },
      complete: () =>{ this._guardandoDatos = false;}
    })
  }

  GuardarImportacionArchivoInversores(){
this._guardandoDatos = true;

    this.#InversoresService.mNuevosInversores(this._Instalacion?.idInstalacion!, this._inversoresImportados).subscribe({
      next: () => {
        this.#toastr.GeneraAlertaToastDirecta("Inversores", "Se han insertado correctamente los inversores.");
        this._inversoresImportados = [];
        this.ObtenerInversores();
      },
      error: (err)=>{ this.#toastr.GeneraAlertaToastDirecta("Error", "Ocurrió un error al insertar los inversores.");
        this._guardandoDatos = false;
       },
      complete: () =>{ this._guardandoDatos = false;}
    })
  }

  guardarPanel(panel: PanelModel){
    const panModif = new PanelModel();
    Object.assign(panModif, panel);

    this._guardandoDatos = true;

    if(panModif.idPanel > 0){
      this.#instalaciones.mModificaPanel(panModif).subscribe({
        next: (res) => {
          if (res.exito) {
            this.#toastr.GeneraAlertaToastDirecta("Paneles", "Se ha actualizado correctamente el panel.");
            this.ObtenPaneles();
          }else{
            this.#toastr.GeneraAlertaToastDirecta("Error", "Error al actualizar el panel: " + res.mensaje);
          }
        },
        error: (err) => {
          this.#toastr.GeneraAlertaToastDirecta("Error", "Ocurrió un error al actualizar el panel.");
          this._guardandoDatos = false;
        },
        complete: () => {
          this._guardandoDatos = false;
        }
      });

    }else{
      panModif.idInstalacion = this._Instalacion?.idInstalacion!;
      this.#instalaciones.mInsertaPaneles(panModif.idInstalacion, [panModif]).subscribe({
        next: (res) => {
          if (res.exito) {
            this.#toastr.GeneraAlertaToastDirecta("Paneles", "Se ha agregado correctamente el panel.");
            this.ObtenPaneles();
          }else{
            this.#toastr.GeneraAlertaToastDirecta("Error", "Error al agregar el panel: " + res.mensaje);
          }
        },
        error: (err) => {
          this.#toastr.GeneraAlertaToastDirecta("Error", "Ocurrió un error al agregar el panel.");
          this._guardandoDatos = false;
        },
        complete: () => {
          this._guardandoDatos = false;
        }
      });

    }
  }

  MostrandoModalPaneles(){
    this._guardandoDatos = true;

    this.ObtenPaneles();

  }

  ObtenPaneles(){
    this.#instalaciones.mObtenPaneles(this._Instalacion?.idInstalacion!).subscribe({
      next: (paneles) => {
        this._paneles = paneles.data;
        this._cantidadPaneles = this._paneles.length;
        this._potenciaPaneles = this._paneles.reduce((acc, p) => acc + p.potencia, 0);
      },
      error: (err)=>{ this.#toastr.GeneraAlertaToastDirecta("Error", "Ocurrió un error al recuperar la información de los paneles.");
        this._guardandoDatos = false;
       },
      complete: () =>{ this._guardandoDatos = false;}
    })
  }

  mEliminarAparato(tipoAparato: string, idAparato: number) {
    this._guardandoDatos = true;

    var obs = tipoAparato === 'I' ? this.#InversoresService.mEliminarInversor(idAparato) : this.#instalaciones.mEliminarPanel(idAparato);

    obs.subscribe({
      next: (res) => {
        if (res.exito) {
          this.#toastr.GeneraAlertaToastDirecta(tipoAparato === 'I' ? "Inversores" : "Paneles", "Se ha eliminado correctamente el " + (tipoAparato === 'I' ? "inversor." : "panel."),NivelAlerta.Exito);
          if(tipoAparato==='I'){
            this.ObtenerInversores();
          }else{
            this.ObtenPaneles();
          }
        } else {
          this.#toastr.GeneraAlertaToastDirecta("Error", "Error al eliminar el " + (tipoAparato === 'I' ? "inversor: " : "panel: ") + res.mensaje, NivelAlerta.Advertencia);
        }
      },
      error: (err) => {
        this.#toastr.GeneraAlertaToastDirecta("Error", "Ocurrió un error al eliminar el " + (tipoAparato === 'I' ? "inversor." : "panel."), NivelAlerta.Peligro);
        this._guardandoDatos = false;
      },
      complete: () => {
        this._guardandoDatos = false;
      }
    });
  }
  
  guardarInversor(inversor: any) {
    const invModif = new InversorModel();
    Object.assign(invModif, inversor);

    this._guardandoDatos = true;

    if(invModif.idInversor > 0){
      this.#InversoresService.mModificaInversor(this._Instalacion?.idInstalacion!, invModif).subscribe({
        next: (res) => {
          if (res.exito) {
            this.#toastr.GeneraAlertaToastDirecta("Inversores", "Se ha actualizado correctamente el inversor.");
            this.ObtenerInversores();
          }else{
            this.#toastr.GeneraAlertaToastDirecta("Error", "Error al actualizar el inversor: " + res.mensaje);
          }
        },
        error: (err) => {
          this.#toastr.GeneraAlertaToastDirecta("Error", "Ocurrió un error al actualizar el inversor.");
          this._guardandoDatos = false;
        },
        complete: () => {
          this._guardandoDatos = false;
        }
      });

    }else{
      invModif.idInstalacion = this._Instalacion?.idInstalacion!;
      this.#InversoresService.mNuevosInversores(this._Instalacion?.idInstalacion!, [invModif]).subscribe({
        next: (res) => {
          if (res.exito) {
            this.#toastr.GeneraAlertaToastDirecta("Inversores", "Se ha agregado correctamente el inversor.");
            this.ObtenerInversores();
          }else{
            this.#toastr.GeneraAlertaToastDirecta("Error", "Error al agregar el inversor: " + res.mensaje);
          }
        },
        error: (err) => {
          this.#toastr.GeneraAlertaToastDirecta("Error", "Ocurrió un error al agregar el inversor.");
          this._guardandoDatos = false;
        },
        complete: () => {
          this._guardandoDatos = false;
        }
      });

    }
  }
  cambiarDeArchivo() {
    this._panelesImportados.length = 0;
    this._inversoresImportados.length = 0;
  }

  calcularPorcentaje() {
    const real = this.generacionReal;
    const garantizada = this.generacionGarantizada;

    if (real == null || garantizada == null || garantizada === 0) {
      this.porcentajeEficacia = 0;
      return;
    }

    this.porcentajeEficacia = (real / garantizada) * 100;
  }


  calcularEstatus() {
    const p = this.porcentajeEficacia ?? 0;

    if (p === 0) {
      this.estatusColor = 'secondary';
      this.estatusTexto = 'Datos insuficientes';
      return;
    }

    if (p >= 90) {
      this.estatusColor = 'success';
      this.estatusTexto = 'Buena generación';
    } else if (p >= 75) {
      this.estatusColor = 'warning';
      this.estatusTexto = 'Generación regular';
    } else {
      this.estatusColor = 'danger';
      this.estatusTexto = 'Necesita atención';
    }
  }


}

