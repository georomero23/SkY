import { state, style, transition, trigger } from '@angular/animations';
import { CommonModule, NgFor } from '@angular/common';
import { Component, inject, NgModule, OnInit, ViewChild } from '@angular/core';
import { ButtonDirective, ButtonGroupComponent, CardBodyComponent, CardComponent, ColComponent, ColDirective, FormModule, InputGroupComponent, ModalBodyComponent, ModalComponent, ModalFooterComponent, ModalHeaderComponent, ModalToggleDirective, RowComponent, SpinnerComponent, TabDirective, TableDirective, TabPaneComponent, TabPanelComponent, TabsComponent, TabsContentComponent, TabsListComponent, TemplateIdDirective, TextColorDirective, WidgetStatFComponent } from '@coreui/angular';
import { IconComponent, IconDirective } from '@coreui/icons-angular';
import { PotenciaRedondeoPipe } from '../../../../pipes/potencia-redondeo.pipe';
import { iconSubset } from '../../../../icons/icon-subset';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { InstalacionGeneracionMensual, TablaGeneracion, TablaGeneracionDiaria } from '@models/dashboard-models';
import { ActivatedRoute, Router, RouterLinkActive } from '@angular/router';
import { DashboardService } from '@services/InformacionEntrePantallas/dashboard.service';
import { FormControlDirective, FormsModule, NgModel } from '@angular/forms';
import { ChartjsComponent } from '@coreui/angular-chartjs';
import { ChartData } from 'chart.js';
import { InversoresService } from '@services/API/inversores-service';
import { InversorDatosAjustados } from '@models/inversor-model';
import { ToastService } from '@services/Front/toast.service';
import { ToastModel } from '@models/toast-model';

@Component({
  selector: 'app-estadisticas-dashboard',
  imports: [CardComponent, CardBodyComponent, RowComponent, ColComponent, ColDirective, ChartjsComponent,
    TabsComponent, IconDirective, TextColorDirective, TabsContentComponent, TabPanelComponent,
    TableDirective, TabDirective, TabsListComponent, NgFor, SpinnerComponent, CommonModule, FormsModule,
    ButtonDirective, ModalComponent, ModalHeaderComponent, ModalBodyComponent, ModalFooterComponent,
    ModalToggleDirective, SpinnerComponent, InputGroupComponent, FormModule],
  templateUrl: './estadisticas-dashboard.component.html',
  styleUrl: './estadisticas-dashboard.component.scss'
})
export class EstadisticasDashboardComponent implements OnInit {
  #routeService = inject(ActivatedRoute);
  #routerService = inject(Router);
  #DashboardService = inject(DashboardService);
  #InstalacionesService = inject(InstalacionesService);
  #InversoresService = inject(InversoresService);
  #ToastrService = inject(ToastService);

  _mesSeleccionado : number = 1;
  _anioSeleccionado! : SelectAnioOption;
  _anioSeleccionado2! : SelectAnioOption;

  _meses: string[] = new Array("Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre");

_anios: SelectAnioOption[] = [];
    iconos = iconSubset;

    _tablaGeneracion:TablaGeneracion|undefined;
    _tablaGeneracionDiaria:TablaGeneracionDiaria|undefined;

    _inicioOperacionesAnio: number = 0;
    _inicioOperacionesMes: number = 0;
    _fechaHoyAnio:number = 0;
    _fechaHoyMes:number = 0;

_cargandoDatos:number = 1;
_idPlataforma:number = 0;

_editandoDatos: (string |undefined)[][] = [];
_guardandoDatos: boolean = false;

  editandoGarantizados = false;
  garantizadoModel: InstalacionGeneracionMensual[] = [];

  data1:ChartData = {labels: this._meses , datasets:[]};
  data2:ChartData = {labels: [] , datasets:[]};

  @ViewChild('Chart1') Chart1!: ChartjsComponent;
  @ViewChild('Chart2') Chart2!: ChartjsComponent;
  @ViewChild('buttonClose') Button!: HTMLButtonElement;

    constructor(){
      if(this.#DashboardService._Instalacion == undefined){
        this.#routerService.navigate(['.'], { relativeTo: this.#routeService.parent });
      }
    }

    ngOnInit(): void {
      this._fechaHoyAnio = new Date().getFullYear();
      this._fechaHoyMes = new Date().getMonth() + 1;
      this._inicioOperacionesAnio = this.#DashboardService._Instalacion!.dtFechaInicioOperaciones.getFullYear();
      this._inicioOperacionesMes = this.#DashboardService._Instalacion!.dtFechaInicioOperaciones.getMonth() + 1;
      this._anios=Array.from({length: 1+(new Date().getFullYear() - this._inicioOperacionesAnio)}, (_,index)=> this._inicioOperacionesAnio + index)
        .sort((a,b)=>a>b?a:b).map(v=> {return {value: v, label: v.toString()}});
      this._idPlataforma = this.#DashboardService._Instalacion!.idPlataforma;
      //if(this._idPlataforma == 5)
        //this.AnioCambiado({target:{value: this._anios[0]}});
      this._anioSeleccionado = this._anios[0];
      this.AnioCambiado();
      //this.GeneracionDiariaCambiada();
    }
    
    AnioCambiado(){
      this._cargandoDatos = 1; //3;

      this.#InstalacionesService.mObtenerTablaGeneracion(this.#DashboardService._Instalacion?.idCliente??0,
         this.#DashboardService._Instalacion?.idInstalacion??0, this._anioSeleccionado.value)
         .subscribe({
            next: (data)=>{
              if(data){
                this._tablaGeneracion = data;
                this.data1.datasets.pop();
                this.data1.datasets.push({
                  label: 'Potencia total Mes (kwH)',
                  data: data.arrValoresReales
                });
                //this.Chart1.chartUpdate()
              }
            },
            error: (ERR)=> {
              console.log(ERR);
              this._cargandoDatos =0;

            },
            complete: ()=>{
              this._cargandoDatos =0;
            }
      });

    }

    GeneracionDiariaCambiada(){
      this._cargandoDatos = 1;

      this.#InstalacionesService.mObtenerDatosDiarios(this.#DashboardService._Instalacion?.idCliente??0,
         this.#DashboardService._Instalacion?.idInstalacion??0, this._anioSeleccionado2.value, +this._mesSeleccionado)
         .subscribe({
            next: (data)=>{
              if(data){
                this._tablaGeneracionDiaria = data;

                // while (this.data2.labels!.length > 0) {
                //   this.data2.labels!.pop();
                // }
                // while (this.data2.datasets.length > 0) {
                //   this.data2.datasets.pop();
                // }
                // for (let i = 0; i < data.datos.length; i++) {
                //   this.data2.labels?.push(data.datos[i].iDia);
                // }
                // for(let i = 0; i< data.inversoresEncabezados.length; i++){
                //   this.data2.datasets.push({
                //     label: 'Potencia '+ data.inversoresEncabezados[i] +' (kwH)',
                //     data: data.datos.map(d=>d.datosInversores[i])
                //   });
                // }
                //this.Chart2.chartUpdate()
                this.ActualizaChart(2);
              }
            },
            error: (ERR)=> {
              console.log(ERR);
              this._cargandoDatos =0;

            },
            complete: ()=>{
              this._cargandoDatos =0;
            }
      });

    }

    GuardarEdicionDatos(){
       let valores : InversorDatosAjustados[] = [];
      
      this._editandoDatos.forEach((value,index)=>{
        if(value){
          value.forEach((value2, index2)=>{
            var nuevoValor = new InversorDatosAjustados();
            nuevoValor.idInstalacion = this.#DashboardService._Instalacion!.idInstalacion;
            nuevoValor.identificadorInversor = this._tablaGeneracionDiaria!.inversoresIdentificadores[index2];
            nuevoValor.fechaRegistro = new Date(this._anioSeleccionado2.value,this._mesSeleccionado-1,index+1).toISOString().slice(0, 10);
            nuevoValor.valorOriginal = this._tablaGeneracionDiaria?.datos[index].datosInversores[index2]??0;
            nuevoValor.valorNuevo = +(value2??"0")
            valores.push(nuevoValor);
          });
        }
      });

      if(valores.length == 0){
        this.#ToastrService.GeneraAlertaToastDirecta("Aviso", "Debe de hacer al menos una modificación para guardar los cambios.");
        return;
      }

      this._guardandoDatos = true;
      this.#InversoresService.mAjustaDatosInversor(valores).subscribe({
        next: (respuesta)=>{
          if(respuesta.exito){
            this._editandoDatos.forEach((value,index)=>{
              if(value){
                value.forEach((value2, index2)=>{
                  this._tablaGeneracionDiaria!.datos[index].datosInversores[index2] = +(value2??"0");
                  this._tablaGeneracionDiaria!.datos[index].arregloEditados[index2] = true;
                });
              }
            });
            this.ActualizaChart(2);

            //Se limpia el arreglo de editandoDatos
            this._editandoDatos.length=0;
            this.#ToastrService.GeneraAlertaToast(new ToastModel("Modificación exitosa", 
              "Los datos de los inversores han sido modificados con éxito."));
            document.getElementById('buttonClose')!.click();
          }
        },
        error:(err)=> {console.log(err);this._guardandoDatos = false;},
        complete: ()=>{
          this._guardandoDatos = false;
        }
      });
    }

    ActualizaChart(cualChart:number){
      if(cualChart == 2){
        let data = this._tablaGeneracionDiaria!;
        while (this.data2.labels!.length > 0) {
          this.data2.labels!.pop();
        }
        while (this.data2.datasets.length > 0) {
          this.data2.datasets.pop();
        }
        for (let i = 0; i < data.datos.length; i++) {
          this.data2.labels?.push(data.datos[i].iDia);
        }
        for(let i = 0; i< data.inversoresEncabezados.length; i++){
          this.data2.datasets.push({
            label: 'Potencia '+ data.inversoresEncabezados[i] +' (kwH)',
            data: data.datos.map(d=>d.datosInversores[i])
          });
        }
        this.Chart2.chartUpdate();
      }

    }

    EditandoValor(i:number, j:number, inv: number|null){
      if(this._editandoDatos[i] == undefined)
        this._editandoDatos[i]=[];
      this._editandoDatos[i][j] = (inv??0).toString()
    }

    comenzarEdicionGarantizados() {
      this.editandoGarantizados = true;
      this.garantizadoModel = [...this._tablaGeneracion!.arrValoresGarantizados];
    }
    
  guardarGarantizados() {
    // Lógica para guardar los valores garantizados (por ejemplo, llamar a tu servicio)

    this.#InstalacionesService.mActualizarGarantizados(this._tablaGeneracion!.idInstalacion, this._anioSeleccionado.value, this.garantizadoModel).subscribe({
      next: (respuesta) => {
        if (respuesta.exito) {
          this._tablaGeneracion!.arrValoresGarantizados = [...this.garantizadoModel];
          this.#ToastrService.GeneraAlertaToast(new ToastModel("Modificación exitosa",
            "Los valores garantizados han sido modificados con éxito."));
        }else{
          this.#ToastrService.GeneraAlertaToast(new ToastModel("Modificación fallida",
            "Los valores garantizados no han podido ser modificados: " + respuesta.mensaje));
        }
      },
      error: (err) => { 
        this.#ToastrService.GeneraAlertaToast(new ToastModel("Modificación fallida",
          "Ocurrió un error al actualizar los valores"));
      },
      complete: () => { 
        this.editandoGarantizados = false;
      }
    });

  }
}


export class SelectAnioOption{
  value: number = 0;
  label: string = "";
}


