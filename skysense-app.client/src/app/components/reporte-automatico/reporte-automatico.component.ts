import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { Component, EventEmitter, inject, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ChartjsComponent } from '@coreui/angular-chartjs';
import { ReporteAutomaticoDatosModel } from '@models/reporteAutomatico-model';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { Chart, ChartData, ChartOptions } from 'chart.js';
import { ZeroToDashPipe } from 'src/app/pipes/zero-to-dash.pipe';
import ChartDataLabels from 'chartjs-plugin-datalabels';

Chart.register(ChartDataLabels);

@Component({
  selector: 'app-reporte-automatico',
  imports: [DecimalPipe, ZeroToDashPipe, ChartjsComponent],
  templateUrl: './reporte-automatico.component.html',
  styleUrl: './reporte-automatico.component.scss'
})
export class ReporteAutomaticoComponent implements OnInit, OnDestroy {

//@ViewChild(ChartjsComponent) chart1: ChartjsComponent | undefined;

  @Input() idInstalacion: number | null = null;
  @Input() iAnno: number | null = null
  @Input() iMes: number | null = null

  @Output() errorEnReporte = new EventEmitter<string>();
  @Output() datosObtenidos = new EventEmitter<{
    panelesGeneracion: number;
    ahorroAcumulado: number;
    ahorroAmbiental: number;
    consumoCFE: number;
  }>();

  #instalacionesService = inject(InstalacionesService);
  _reportData: ReporteAutomaticoDatosModel | null = null;

  _barChartData: ChartData<'bar'> | undefined;
  _barChartData2: ChartData<'bar'> | undefined;
  _barChartData3: ChartData<'bar'> | undefined;
  _lineChartData: ChartData<'line'> | undefined;
  _lineChartData2: ChartData<'line'> | undefined;

  _barChartOptions: ChartOptions<'bar'> = {};
  _barChartOptions2: ChartOptions<'bar'> = {};
  _barChartOptions3: ChartOptions<'bar'> = {};
  _lineChartOptions: ChartOptions<'line'> = {};
  _lineChartOptions2: ChartOptions<'line'> = {};

  chart1: Chart | undefined;
  chart2: Chart | undefined;
  chart3: Chart | undefined;
  chart4: Chart | undefined;
  chart5: Chart | undefined;

  constructor() { }

  ngOnInit(): void {
    if (this.idInstalacion && this.iAnno && this.iMes) {
      this.#instalacionesService.obtenerReporteAutomatico(this.idInstalacion, this.iAnno, this.iMes).subscribe({
        next: (data) => {
          if (!data.exito || !data.data) {
            this.errorEnReporte.emit((data.mensaje || 'Error desconocido'));
            return;
          }
        this._reportData = data.data;
        this.datosObtenidos.emit({
          panelesGeneracion: this._reportData?.generacionPeriodo || 0,
          ahorroAcumulado: this._reportData?.ahorroAcumuladoSIva || 0,
          ahorroAmbiental: this._reportData?.aporteCO2 || 0,
          consumoCFE: this._reportData?.consumoCFE || 0
        });

        this.ConfigurarGraficaCumplimiento();
        this.ConfiguracionGrafica2();
        this.ConfiguracionGrafica3();
        this.ConfiguracionGrafica4();
        this.ConfiguracionGrafica5();

      }, error: err => {
        console.error('Error al obtener el reporte automático:', err);
        this.errorEnReporte.emit('Error al obtener el reporte automático: ' + (err?.error?.message || err?.message || 'Error desconocido'));
      }
    });
    }
  }


  // Gráfica de barras
  ConfigurarGraficaCumplimiento() {
    const config = {
        type: 'bar',
        data: [this._reportData?.porcentajeCumplimiento],
        options: {
          indexAxis: 'y',
          maintainAspectRatio: false,
          scales: {
            x:{
              min: 0,
              ticks: 20
            }
          }
        }
    }

    this._barChartData = {
      datasets: [
        {
          data: [this._reportData?.porcentajeCumplimiento || 0],
          barThickness: 10,
          backgroundColor: '#00b0f0'
        }
      ],labels: ['']}

    this._barChartOptions = {
      indexAxis: 'y',
      maintainAspectRatio: false,
      responsive: true,
      plugins: {datalabels: { display: false }, legend: { display: false } },
      scales:{
        'x':{
          min: 0,
          suggestedMax: 120  ,
          ticks: {
            callback: (value: any, index: any, ticks: any) => {
              return value + '%';
            }
          }
        }
      }
    }

    this.chart1!.update();
  }

  ConfiguracionGrafica2() {
    this._barChartData2 = {
      datasets: [
      {
        data: this._reportData?.generacionDiaria || [0],
        barThickness: 4,
        backgroundColor: '#000000',
      }
      ],
      labels: Array.from({ length: new Date(this.iAnno!, this.iMes!, 0).getDate() }, (_, i) => (i + 1).toString())
    }

    this._barChartOptions2 = {
      maintainAspectRatio: false,
      responsive: true,
      // font: {
      //   size: 5
      // },
      plugins: { datalabels: { display: false }, legend: { display: false } },
      scales:{
        'y':{
          ticks:{
            font:{
              size:8
            }
          }
        },
        'x':{
          ticks:{
            font:{
              size:6
            },
            maxRotation:0, // Maximum rotation in degrees
          minRotation: 0
            ,autoSkip: false
          },
          grid:{
            display:false
          }
        }
      }
    }

    this.chart2!.update();
  }

  ConfiguracionGrafica3() {
    this._barChartData3 = {
      datasets: [
      {
        data: [this._reportData?.consumoTotal||0],
        barThickness: 40,
        backgroundColor: '#0f0957',
        datalabels: {
          color: '#0f0957',
        },
        label: 'Consumo total'
      },
      {
        data:[this._reportData?.consumoCFE || 0],
        barThickness: 40,
        backgroundColor: '#0070c0',
        datalabels: {
          color: '#0070c0',
        },
        label: 'Consumo CFE'
      },
      {
        data:[this._reportData?.consumoPaneles || 0],
        barThickness: 40,
        backgroundColor: '#00b0f0',
        datalabels: {
          color: '#00b0f0',
        },
        label: 'Consumo Paneles'
      }
      ],
      labels: ['Consumo']
    }

    this._barChartOptions3 = {
      maintainAspectRatio: false,
      responsive: true,
      
      // font: {
      //   size: 5
      // },
      plugins: {
        datalabels: {
          anchor: 'end',
          align: 'top',
          font: {
            size: 10,
            weight: 'bold'
          },
          formatter: function (value) {
            return value;
          }
        },
        legend: { 
          display: true,
          position: 'bottom',
          labels:{
            font:{
              size:9
            },
            boxWidth:9
          }
         }
    },
      scales:{
        'y':{
          ticks:{
            font:{
              size:9
            }
          },
          max: Math.round(Math.max(this._reportData?.consumoTotal||0, this._reportData?.consumoCFE || 0, this._reportData?.consumoPaneles || 0) * 1.2/1000)*1000
        },
        'x':{
          ticks:{
            display:false
          },
          grid:{
            display:false
          }
        }
      }
    }

    this.chart3!.update();
  }

  ConfiguracionGrafica4() {
    this._lineChartData = {
      datasets: [
      {
        data: this._reportData?.historicoGenPVAnnioAnterior||[0],
        borderColor: '#ffc702',
        borderDash: [5,5],
        //borderWidth: 2,
        label: '2024'
      },
      {
        data:this._reportData?.historicoGenPVEsteAnnio || [0],
        borderColor: '#ffc702',
        //borderWidth: 2,
        label: '2025'
      }
      ],
      labels: ['Enero','Febrero','Marzo','Abril','Mayo','Junio','Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre']
    }

    this._lineChartOptions = {
      maintainAspectRatio: false,
      responsive: true,
      
      // font: {
      //   size: 5
      // },
      plugins: {
        datalabels: {
          display: false
        },
        legend: { 
          display: true,
          position: 'bottom',
          labels:{
            font:{
              size:14
            }
          }
         }
    },
      scales:{
        'y':{
          ticks:{
            font:{
              size:9
            },
          },
          beginAtZero: true
        },
        'x':{
          grid:{

            display:false
          }
        }
      }
    }

    this.chart4!.update();
  }

  ConfiguracionGrafica5() {
    this._lineChartData2 = {
      datasets: [
      {
        data: this._reportData?.historicoConsumo||[null],
        borderColor: '#0072c3',
        label: 'Consumo '+this.iAnno,
        order:0,
        yAxisID: 'y1'
      },
      {
        data: this._reportData?.historicoFacturas[0] || [null],
        borderColor: '#00b854',
        //borderWidth: 2,
        label: 'Total '+this.iAnno,
        order:1,
        yAxisID: 'y2'
      },
      {
        data: this._reportData?.historicoFacturas[1] || [null],
        borderColor: '#00b85499',
        borderDash: [5,3],
        //borderWidth: 2,
        label: 'Total '+(this.iAnno!-1),
        order:2,
        yAxisID: 'y2'
      },
      {
        data: this._reportData?.historicoFacturas[2] || [null],
        borderColor: '#00b85444',
        borderDash: [5,3],
        //borderWidth: 2,
        label: 'Total '+(this.iAnno!-2),
        order:3,
        yAxisID: 'y2'
      }
      ],
      labels: ['Enero','Febrero','Marzo','Abril','Mayo','Junio','Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre']
    }

    this._lineChartOptions2 = {
      maintainAspectRatio: false,
      responsive: true,
      
      // font: {
      //   size: 5
      // },
      plugins: {
        datalabels: {
          display: false
        },
        legend: { 
          display: true,
          position: 'bottom',
          labels:{
            font:{
              size:14
            }
          }
         }
    },
      scales:{
        'y1':{
          ticks:{
            font:{
              size:9
            },
          },
          beginAtZero: true,
          position: 'left'
        },
        'y2':{
          ticks:{
            font:{
              size:9
            },
            callback: (value: any, index: any, ticks: any) => {
              return '$'+value;
            }
          },
          position: 'right',
          beginAtZero: true
        },
        'x':{
          grid:{

            display:false
          }
        }
      }
    }

    this.chart5!.update();
  }

  ChartRefChanged(numeroGrafica:number, $event: any) {
    switch (numeroGrafica) {
      case 1:
        this.chart1 = $event;
        break;
      case 2:
        this.chart2 = $event;
        break;
      case 3:
        this.chart3 = $event;
        break;
      case 4:
        this.chart4 = $event;
        break;
      case 5:
        this.chart5 = $event;
        break;
      default:        break;
    }
    //this.ConfigurarGraficaCumplimiento();
  }

  ngOnDestroy(): void {

    this.chart1?.destroy();
    this.chart2?.destroy();
    this.chart3?.destroy();
    this.chart4?.destroy();
    this.chart5?.destroy();

    Chart.unregister(ChartDataLabels);
  }
}
