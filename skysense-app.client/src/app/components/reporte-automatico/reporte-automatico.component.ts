import { CommonModule, CurrencyPipe, DecimalPipe } from '@angular/common';
import { Component, EventEmitter, inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ChartjsComponent } from '@coreui/angular-chartjs';
import { ReporteAutomaticoDatosModel, ReporteAutomaticoConfig } from '@models/reporteAutomatico-model';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { Chart, ChartData, ChartOptions } from 'chart.js';
import { ZeroToDashPipe } from 'src/app/pipes/zero-to-dash.pipe';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { SimpleChanges, OnChanges, ChangeDetectorRef } from '@angular/core';

Chart.register(ChartDataLabels);

@Component({
  selector: 'app-reporte-automatico',
  standalone: true,
  imports: [
    CommonModule,         
    DecimalPipe,
    CurrencyPipe,
    ZeroToDashPipe,
    ChartjsComponent
  ],
  templateUrl: './reporte-automatico.component.html',
  styleUrl: './reporte-automatico.component.scss'
})
export class ReporteAutomaticoComponent implements OnInit, OnDestroy, OnChanges {


  @Input() idInstalacion: number | null = null;
  @Input() iAnno: number | null = null;
  @Input() iMes: number | null = null;

  private _mostrarBT2: boolean = false;
  @Input() set mostrarBT2(value: any) {
    this._mostrarBT2 = this.toBoolean(value);
  }
  get mostrarBT2(): boolean {
    return this._mostrarBT2;
  }
  constructor(private cd: ChangeDetectorRef) { }

  ngOnChanges(changes: SimpleChanges): void {
  if (changes['mostrarBT2']) {
    this.cd.detectChanges();  
  }
}


  @Output() errorEnReporte = new EventEmitter<string>();
  @Output() datosObtenidos = new EventEmitter<{
    panelesGeneracion: number;
    ahorroAcumulado: number;
    ahorroAmbiental: number;
    consumoCFE: number;
  }>();

  #instalacionesService = inject(InstalacionesService);

  _reportData: ReporteAutomaticoDatosModel | null = null;


  _barChartData!: ChartData<'bar'>;
  _barChartData2!: ChartData<'bar'>;
  _barChartData3!: ChartData<'bar'>;
  _lineChartData!: ChartData<'line'>;
  _lineChartData2!: ChartData<'line'>;

  _barChartOptions: ChartOptions<'bar'> = {};
  _barChartOptions2: ChartOptions<'bar'> = {};
  _barChartOptions3: ChartOptions<'bar'> = {};
  _lineChartOptions: ChartOptions<'line'> = {};
  _lineChartOptions2: ChartOptions<'line'> = {};

  chart1?: Chart;
  chart2?: Chart;
  chart3?: Chart;
  chart4?: Chart;
  chart5?: Chart;

  ngOnInit(): void {
    if (!this.idInstalacion || !this.iAnno || !this.iMes) return;

    this.#instalacionesService.obtenerReporteAutomatico(this.idInstalacion, this.iAnno, this.iMes)
      .subscribe({
        next: (resp) => {
          if (!resp.exito || !resp.data) {
            this.errorEnReporte.emit(resp.mensaje || "Error desconocido");
            return;
          }

          this._reportData = resp.data;

          this.datosObtenidos.emit({
            panelesGeneracion: this._reportData.generacionPeriodo,
            ahorroAcumulado: this._reportData.ahorroAcumuladoSIva,
            ahorroAmbiental: this._reportData.aporteCO2,
            consumoCFE: this._reportData.consumoCFE
          });

          this.ConfigurarGraficaCumplimiento();
          this.ConfiguracionGrafica2();
          this.ConfiguracionGrafica3();
          this.ConfiguracionGrafica4();
          this.ConfiguracionGrafica5();
        },
        error: err => {
          console.error(err);
          this.errorEnReporte.emit("Error al obtener el reporte automático");
        }
      });
  }

  ChartRefChanged(numero: number, event: any) {
    switch (numero) {
      case 1:
        this.chart1 = event;
        break;
      case 2:
        this.chart2 = event;
        break;
      case 3:
        this.chart3 = event;
        break;
      case 4:
        this.chart4 = event;
        break;
      case 5:
        this.chart5 = event;
        break;
    }
  }

  ConfigurarGraficaCumplimiento() {
    this._barChartData = {
      datasets: [{
        data: [this._reportData?.porcentajeCumplimiento || 0],
        barThickness: 10,
        backgroundColor: '#00b0f0'
      }],
      labels: ['']
    };

    this._barChartOptions = {
      indexAxis: 'y',
      responsive: true,
      maintainAspectRatio: false,
      plugins: { legend: { display: false }, datalabels: { display: false } },
      scales: {
        x: {
          min: 0,
          suggestedMax: 120,
          ticks: { callback: (v) => v + '%' }
        }
      }
    };
  }

  ConfiguracionGrafica2() {
    this._barChartData2 = {
      datasets: [{
        data: this._reportData?.generacionDiaria || [],
        backgroundColor: '#000'
      }],
      labels: Array.from(
        { length: new Date(this.iAnno!, this.iMes!, 0).getDate() },
        (_, i) => (i + 1).toString()
      )
    };

    this._barChartOptions2 = {
      responsive: true,
      maintainAspectRatio: false,
      plugins: { legend: { display: false }, datalabels: { display: false } }
    };
  }

  ConfiguracionGrafica3() {
    this._barChartData3 = {
      datasets: [
        {
          data: [this._reportData?.consumoTotal || 0],
          backgroundColor: '#0f0957',
          label: 'Consumo total'
        },
        {
          data: [this._reportData?.consumoCFE || 0],
          backgroundColor: '#0070c0',
          label: 'Consumo CFE'
        },
        {
          data: [this._reportData?.consumoPaneles || 0],
          backgroundColor: '#00b0f0',
          label: 'Consumo Paneles'
        }
      ],
      labels: ['Consumo']
    };

    this._barChartOptions3 = {
      responsive: true,
      maintainAspectRatio: false
    };
  }

  ConfiguracionGrafica4() {
    this._lineChartData = {
      datasets: [
        {
          data: this._reportData?.historicoGenPVAnnioAnterior || [],
          borderColor: '#ffc702',
          borderDash: [5, 5],
          label: (this.iAnno! - 1).toString()
        },
        {
          data: this._reportData?.historicoGenPVEsteAnnio || [],
          borderColor: '#ffc702',
          label: this.iAnno!.toString()
        }
      ],
      labels: [
        'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
        'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
      ]
    };

    this._lineChartOptions = {
      responsive: true,
      maintainAspectRatio: false
    };
  }

  ConfiguracionGrafica5() {
    this._lineChartData2 = {
      datasets: [
        {
          data: this._reportData?.historicoConsumo || [],
          borderColor: '#0072c3',
          label: 'Consumo ' + this.iAnno,
          yAxisID: 'y1'
        },
        ...[0, 1, 2].map(i => ({
          data: this._reportData?.historicoFacturas[i] || [],
          borderColor: i === 0 ? '#00b854' : '#00b85455',
          borderDash: i === 0 ? [] : [5, 3],
          label: 'Total ' + (this.iAnno! - i),
          yAxisID: 'y2'
        }))
      ],
      labels: [
        'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
        'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
      ]
    };

    this._lineChartOptions2 = {
      responsive: true,
      maintainAspectRatio: false
    };
  }

  ngOnDestroy(): void {
    this.chart1?.destroy();
    this.chart2?.destroy();
    this.chart3?.destroy();
    this.chart4?.destroy();
    this.chart5?.destroy();
    Chart.unregister(ChartDataLabels);
  }

  private toBoolean(value: any): boolean {
    if (typeof value === 'string') {
      value = value.trim().toLowerCase();
    }

    return value === true || value === 'true' || value === 1 || value === '1';
  }

}
