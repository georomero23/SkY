import { Component, inject, OnInit, ViewChildren, QueryList, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { OPCBateria, TagBateria } from '@models/baterias-models';
import { BateriasService } from '@services/API/baterias.service';
import { SignalrService } from '@services/Front/signalr.service';
import { ToastService } from '@services/Front/toast.service';
import { RowComponent, ColComponent, CardModule } from "@coreui/angular";
import { ChartjsComponent } from '@coreui/angular-chartjs';
import { CommonModule } from '@angular/common';
import 'chartjs-adapter-date-fns';

@Component({
  selector: 'app-bateria-real-time',
  imports: [RowComponent, ColComponent, CardModule, ChartjsComponent, CommonModule],
  templateUrl: './bateria-real-time.component.html',
  styleUrl: './bateria-real-time.component.scss'
})
export class BateriaRealTimeComponent implements OnInit, OnDestroy {
  #bateriasService = inject(BateriasService);
  #signalRService = inject(SignalrService);
  #tostadaService = inject(ToastService);
  #routeService = inject(ActivatedRoute);

  hub : HubConnection;

  idInstalacion: number = 0;

  configuracionBateria: OPCBateria = new OPCBateria();

  // referencias a los c-charts
  @ViewChildren('chartRef') chartRefs!: QueryList<any>;

  // arrays por tag
  tagsGraficables: any[] = [];
  tagsNoGraficables: any[] = [];
  tagsControl: any[] = [];
  chartData: any[][] = [];
  //chartLabels: string[][] = [];

  // opciones compartidas (ajusta según necesites)
  chartOptions: any = {
    maintainAspectRatio: false,
    
    responsive: true,
    plugins: { legend: { display: false } },
    scales: {
      y: { beginAtZero: true },
      x: {
        type: 'time',
        time: {
          unit: 'second',
          tooltipFormat: 'HH:mm:ss',
          displayFormats: { second: 'HH:mm:ss' }
        }
      }
    }
  };

  // mantén un límite de puntos por gráfico
  maxPoints = 100;

  constructor() {
    console.log("ID Instalació");
    this.idInstalacion = Number(this.#routeService.snapshot.paramMap.get('idInstalacion'));
    console.log("ID Instalación:", this.idInstalacion);
    this.hub = this.#signalRService.InicializaSignalR();
  }

  ngOnInit() {
    console.log("ID Instalación:", this.idInstalacion);
    this.#bateriasService.ObtenConfiguracionBateria(this.idInstalacion).subscribe({
      next: (config) => {
        if (config.exito) {
          this.configuracionBateria = config.data;
          // inicializa tags graficables y arreglos
          this.tagsGraficables = this.configuracionBateria.tagsBateria.filter((t:any) => t.graficable);
          this.tagsNoGraficables = this.configuracionBateria.tagsBateria.filter((t:TagBateria) => !t.graficable && t.soloLectura);
          this.tagsControl = this.configuracionBateria.tagsBateria.filter((t:TagBateria) => !t.graficable && !t.soloLectura);

          this.chartData = this.tagsGraficables.map(() => []);
          //this.chartLabels = this.tagsGraficables.map(() => []);
          this.ConfigurarMonitoreo();
        }else{
          this.#tostadaService.mostrarError("Error al obtener la configuración de la batería: " + config.mensaje);
        }
      },
      error: (err) => {
        this.#tostadaService.mostrarError("Error al obtener la configuración de la batería: " + err);
      }
    });
  }

  ConfigurarMonitoreo() {
    this.hub.start()
      .then(() => console.log('Hub connection started'))
      .then(() => {
        this.hub.invoke('MonitorearBateria', this.idInstalacion)
          .then(() => console.log('Joined BateriasGroup'))
          .catch(err => console.error('Error joining group:', err))
      })
      .catch(err => console.error('Error starting Hub connection:', err));

    this.hub.on('MonitoreoBateria', (data: OPCBateria) => {
      try {
        // por cada tag recibido, ubica su índice en tagsGraficables y añade mediciones
        data.tagsBateria.forEach(tagRec => {
          const idx = this.tagsGraficables.findIndex(t => t.idTag === tagRec.idTag);
          if (idx === -1) return;

          const nuevos = tagRec.mediciones.map((m:any) => { return {x: new Date(m.timestamp), y: Number(m.medicion)} });
          //const nuevasEtiquetas = tagRec.mediciones.map((m:any) => new Date(m.timestamp).toLocaleTimeString());

          // push punto por punto para respetar maxPoints
          nuevos.forEach((valor, k) => {
            this.chartData[idx].push(valor);
            //this.chartLabels[idx].push(nuevasEtiquetas[k]);
            // acota longitud
            if (this.chartData[idx].length > this.maxPoints) {
              this.chartData[idx].shift();
              //this.chartLabels[idx].shift();
            }
          });

          this.actualizarGrafica(idx);
        });
      } catch(err) {
        console.error('Error processing data:', err);
      }
    });
  }

  actualizarGrafica(idx: number) {
    const charts = this.chartRefs ? this.chartRefs.toArray() : [];
    const chartComp = charts[idx];
    if (!chartComp) {
      // si el componente aún no está renderizado, reintentar breve
      setTimeout(() => this.actualizarGrafica(idx), 50);
      return;
    }

    // c-chart suele exponer la instancia real en .chart
    const chartInstance = chartComp.chart ?? chartComp;
    if (chartInstance?.data && chartInstance?.update) {
      //chartInstance.data.labels = [...this.chartLabels[idx]];
      // chartInstance.data.datasets = [{
      //   label: this.tagsGraficables[idx].nombre ?? (`Tag ${this.tagsGraficables[idx].idTag}`),
      //   data: [...this.chartData[idx]],
      //   borderColor: this.tagsGraficables[idx].color ?? '#198754',
      //   backgroundColor: 'transparent',
      //   fill: false,
      //   tension: 0.2,
      //   pointRadius: 0.5
      // }];
      chartInstance.data.datasets[0].data = [...this.chartData[idx]];
      chartInstance.data.datasets[0].borderColor = this.tagsGraficables[idx].color ?? '#198754';
      chartInstance.data.datasets[0].backgroundColor = 'transparent';
      chartInstance.data.datasets[0].fill = false;
      chartInstance.data.datasets[0].tension = 0.2;//la curvatura de la línea 0 es recta, 1 es muy curva
      chartInstance.data.datasets[0].pointRadius = 1;// tamaño del punto 
      chartInstance.data.datasets[0].borderWidth = 2;// grosor de la línea
      chartInstance.update();
    }
  }

  ngOnDestroy() {
    this.hub.stop().then(() => console.log('Hub connection stopped'));
  }
}
