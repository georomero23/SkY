import { CommonModule, DOCUMENT, NgStyle } from '@angular/common';
import { Component, DestroyRef, effect, inject, OnInit, Renderer2, signal, WritableSignal } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ChartDataset, ChartOptions, PluginOptionsByType, ScaleOptions, TooltipLabelStyle } from 'chart.js';
import { AvatarComponent, ButtonDirective, ButtonGroupComponent, CardBodyComponent, CardComponent, CardFooterComponent, CardHeaderComponent, ColComponent, FormCheckLabelDirective, GutterDirective, ProgressBarDirective, ProgressComponent, RowComponent, TableDirective, TextColorDirective, FormModule } from '@coreui/angular';
import { ChartjsComponent } from '@coreui/angular-chartjs';
import { IconDirective } from '@coreui/icons-angular';

import { WidgetsBrandComponent } from '../widgets/widgets-brand/widgets-brand.component';
import { WidgetsDropdownComponent } from '../widgets/widgets-dropdown/widgets-dropdown.component';
import { DashboardChartsData, IChartProps } from './dashboard-charts-data';
import { getStyle } from '@coreui/utils';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { ToastService } from '@services/Front/toast.service';
import { KPIInstalaciones } from '@models/tablas-model';
import { DeepPartial } from 'chart.js/dist/types/utils';
import { GrupoModel } from '@models/instalacion-model';

interface IUser {
  name: string;
  state: string;
  registered: string;
  country: string;
  usage: number;
  period: string;
  payment: string;
  activity: string;
  avatar: string;
  status: string;
  color: string;
}



@Component({
    templateUrl: 'dashboard.component.html',
    styleUrls: ['dashboard.component.scss'],
    imports: [CommonModule, TextColorDirective, CardComponent, CardBodyComponent, RowComponent, ColComponent, ButtonDirective, IconDirective, ReactiveFormsModule,
    ButtonGroupComponent, FormCheckLabelDirective, ChartjsComponent, NgStyle, CardFooterComponent, GutterDirective, ProgressBarDirective, ProgressComponent,
    WidgetsBrandComponent, CardHeaderComponent, TableDirective, AvatarComponent, FormsModule, FormModule]
})
export class DashboardComponent implements OnInit {

  readonly #destroyRef: DestroyRef = inject(DestroyRef);
  readonly #document: Document = inject(DOCUMENT);
  readonly #renderer: Renderer2 = inject(Renderer2);
  readonly #chartsData: DashboardChartsData = inject(DashboardChartsData);
  readonly #instalacionService: InstalacionesService = inject(InstalacionesService);
  readonly #tostada: ToastService = inject(ToastService);

  porcentajeEficacia: number | null = null;
  generacionReal: number | null = null;
  generacionGarantizada: number | null = null;


  grupos: GrupoModel[] = [];
  grupoSeleccionado: string = "-1";

  public Annos: number[] = [2025, 2024, 2023, 2022, 2021, 2020];
  public Meses: {Mes:number, Nombre:string}[] = [
    {Mes: 1, Nombre: 'Enero'},
    {Mes: 2, Nombre: 'Febrero'},
    {Mes: 3, Nombre: 'Marzo'},
    {Mes: 4, Nombre: 'Abril'},
    {Mes: 5, Nombre: 'Mayo'},
    {Mes: 6, Nombre: 'Junio'},
    {Mes: 7, Nombre: 'Julio'},
    {Mes: 8, Nombre: 'Agosto'},
    {Mes: 9, Nombre: 'Septiembre'},
    {Mes: 10, Nombre: 'Octubre'},
    {Mes: 11, Nombre: 'Noviembre'},
    {Mes: 12, Nombre: 'Diciembre'}
  ];

  public instalacionesToditas: KPIInstalaciones[] = [];
  public instalacionesElegidasPorGrupo: KPIInstalaciones[] = [];
  public instalacionesElegidasPorCheck: KPIInstalaciones[] = [];

  public annoSeleccionado: number = new Date().getFullYear();
  public mesSeleccionado: {Mes:number, Nombre:string} = this.Meses[new Date().getMonth()];

  public mainChart: IChartProps = { type: 'bar' };
  public mainChartRef: WritableSignal<any> = signal(undefined);
  #mainChartRefEffect = effect(() => {
    if (this.mainChartRef()) {
      this.setChartStyles();
    }
  });
  public chart: Array<IChartProps> = [];
  public trafficRadioGroup = new FormGroup({
    trafficRadio: new FormControl('Año')
  });


  ngOnInit(): void {
    this.#instalacionService.mObtenerGrupos().subscribe({
      next: (data) => {
        if(data.exito){
          this.grupos = data.data;
        } else {
          this.#tostada.mostrarError('Error al recuperar los grupos: "' + data.mensaje);
        }
      },
      error: () => { this.#tostada.mostrarError('Error al recuperar los grupos'); },
      complete: () => {}
    });
    this.CambioDeValores(false, this.annoSeleccionado, null);
  }

  filtrarPorGrupo() {
    if(this.grupoSeleccionado === "-1"){
      this.instalacionesElegidasPorGrupo = [...this.instalacionesToditas];
    } else {
      const idGrupoSeleccionado = parseInt(this.grupoSeleccionado, 10);
      this.instalacionesElegidasPorGrupo = this.instalacionesToditas.filter(inst => inst.idGrupo === idGrupoSeleccionado);
    }
    this.instalacionesElegidasPorCheck = [...this.instalacionesElegidasPorGrupo];
    this.mainChart = this.CambiarChartPrincipal(this.instalacionesElegidasPorCheck.filter(d=>d.porcentaje != null));
    //this.setChartStyles();
    this.updateChartOnColorModeChange();
  }

  setTrafficPeriod(value: string): void {
    this.trafficRadioGroup.setValue({ trafficRadio: value });
    // this.#chartsData.initMainChart(value);
    // this.initCharts();
    this.CambioDeValores(value==="Garantia", this.annoSeleccionado, value==="Mes" ? this.mesSeleccionado.Mes : null);
  }

  handleChartRef($chartRef: any) {
    if ($chartRef) {
      this.mainChartRef.set($chartRef);
    }
  }

  updateChartOnColorModeChange() {
    const unListen = this.#renderer.listen(this.#document.documentElement, 'ColorSchemeChange', () => {
      this.setChartStyles();
    });

    this.#destroyRef.onDestroy(() => {
      unListen();
    });
  }

  setChartStyles() {
    if (this.mainChartRef()) {
      setTimeout(() => {
        const options: ChartOptions = { ...this.mainChart.options };
        const scales = this.#chartsData.getScales();
        this.mainChartRef().options.scales = { ...options.scales, ...scales };
        this.mainChartRef().update();
      });
    }
  }

  CambioDeValores(annoGarantia: boolean, valorAnno: number, valorMes: number|null) {
    this.#instalacionService.ObtenKPIInstalaciones(annoGarantia, valorAnno, valorMes).subscribe({
      next: (data) => {
        if(data.exito){
          this.instalacionesToditas = data.data.sort((a, b) =>{
            if (a.porcentaje === null && b.porcentaje === null) return 0;
            if (a.porcentaje === null) return 1;
            if (b.porcentaje === null) return -1;
            return a.porcentaje - b.porcentaje;
          });
          this.filtrarPorGrupo();
          // this.mainChart = this.CambiarChartPrincipal(data.data.filter(d=>d.porcentaje != null));
          // this.updateChartOnColorModeChange();
        }else{
          this.#tostada.mostrarError('Error al recuperar los datos: "' + data.mensaje);
        }
      },
      error: (error) => {
        console.error('Error al cambiar valores:', error);
      }
    });
  }

  abrirInstalacion(inst: KPIInstalaciones) {
    this.#instalacionService._UltimaSeleccion = inst;  // Guardamos toda la info del dashboard
    window.location.href = `#/clientes/${inst.idCliente}/${inst.idInstalacion}`;
  }


  CambiarChartPrincipal(dataSet: KPIInstalaciones[]):IChartProps {
    const brandSuccess = getStyle('--cui-success') ?? '#4dbd74';
    const brandInfo = getStyle('--cui-info') ?? '#20a8d8';
    const brandInfoBg = `rgba(${getStyle('--cui-info-rgb')}, .1)`
    const brandDanger = getStyle('--cui-danger') ?? '#f86c6b';

    const chartNuevo:IChartProps = { type: 'bar' };

    // mainChart
    chartNuevo['elements'] = dataSet.length;
    chartNuevo['Data1'] = [];//Para los valores
    chartNuevo['Data2'] = [];//Para la linea
    // chartNuevo['Data3'] = [];

    // generate random values for mainChart
    for (let i = 0; i < chartNuevo['elements']; i++) {
      chartNuevo['Data1'].push(dataSet[i].porcentaje);
      // chartNuevo['Data2'].push(this.random(20, 160));
      chartNuevo['Data2'].push(90);
    }

    let labels: string[] = [];
    labels = dataSet.map(d=>d.nombre);

    const colors = [
      {
        // brandInfo
        backgroundColor: brandInfoBg,
        borderColor: brandInfo,
        pointHoverBackgroundColor: brandInfo,
        borderWidth: 2,
        fill: true
      },
      {
        // brandDanger
        backgroundColor: 'transparent',
        borderColor: brandDanger || '#f86c6b',
        pointHoverBackgroundColor: brandDanger,
        borderWidth: 1,
        borderDash: [8, 5]
      }
    ];

    const datasets: ChartDataset[] = [
      {
        type: 'bar',
        data: chartNuevo['Data1'],
        label: 'Eficacia %',
        ...colors[0]
      },
      {
        type: 'line',
        data: chartNuevo['Data2'],
        label: 'Límite %',
        ...colors[1]
      },
      // {
      //   data: chartNuevo['Data3'],
      //   label: 'BEP',
      //   ...colors[2]
      // }
    ];

    const plugins: DeepPartial<PluginOptionsByType<any>> = {
      legend: {
        display: false
      },
      tooltip: {
        callbacks: {
          labelColor: (context) => ({ backgroundColor: context.dataset.borderColor } as TooltipLabelStyle)
        }
      }
    };

    const scales = this.getScales();

    const options: ChartOptions = {
      maintainAspectRatio: false,
      plugins,
      scales,
      elements: {
        line: {
          tension: 0.4
        },
        point: {
          radius: 0,
          hitRadius: 10,
          hoverRadius: 4,
          hoverBorderWidth: 3
        }
      }
    };

    chartNuevo.type = 'bar';
    chartNuevo.options = options;
    chartNuevo.data = {
      datasets,
      labels
    };

    return chartNuevo;
  }
  getScales() {
      const colorBorderTranslucent = getStyle('--cui-border-color-translucent');
      const colorBody = getStyle('--cui-body-color');
  
      const scales: ScaleOptions<any> = {
        x: {
          grid: {
            color: colorBorderTranslucent,
            drawOnChartArea: false
          },
          ticks: {
            color: colorBody,
            autoskip: false
          }
        },
        y: {
          border: {
            color: colorBorderTranslucent
          },
          grid: {
            color: colorBorderTranslucent
          },
          //max: 150,
          beginAtZero: true,
          ticks: {
            color: colorBody,
            maxTicksLimit: 5
          }
        }
      };
      return scales;
  }

  
  // Switchea(index: number, evento: Event) {
  //   console.log(evento);
  //   const check = evento.target as HTMLInputElement;
  //   this.mainChart.data!.datasets[index].hidden = !check.checked;
  //   this.mainChartRef()?.update();
  // }
}
