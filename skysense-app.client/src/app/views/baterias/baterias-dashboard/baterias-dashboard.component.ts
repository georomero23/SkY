import { Component, OnInit, ViewChild } from '@angular/core';
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';
import { ChartjsComponent } from "@coreui/angular-chartjs";
import { OPCBateria } from '@models/baterias-models';

@Component({
  selector: 'app-baterias-dashboard',
  imports: [ChartjsComponent],
  templateUrl: './baterias-dashboard.component.html',
  styleUrl: './baterias-dashboard.component.scss'
})
export class BateriasDashboardComponent implements OnInit {
  hub:HubConnection;
  public chartData: number[][] = [];
  public chartLabels: string[][] = [];
  @ViewChild('chartRef1') chartRef1: any;
  @ViewChild('chartRef2') chartRef2: any;
  @ViewChild('chartRef3') chartRef3: any;
  @ViewChild('chartRef4') chartRef4: any;
  @ViewChild('chartRef5') chartRef5: any;
  @ViewChild('chartRef6') chartRef6: any;
  @ViewChild('chartRef7') chartRef7: any;
  @ViewChild('chartRef8') chartRef8: any;
  @ViewChild('chartRef9') chartRef9: any;


  constructor(){
    this.hub = new HubConnectionBuilder()
      .withUrl('/api/skyhub')
      .withAutomaticReconnect()
      .build();

    this.chartData = Array(9).fill(null).map(() => []);
    this.chartLabels = Array(9).fill(null).map(() => []);
  }

  ngOnInit() {
    const  charts = [this.chartRef1,this.chartRef2, this.chartRef3, this.chartRef4, this.chartRef5, this.chartRef6, this.chartRef7, this.chartRef8, this.chartRef9];

    this.hub.start()
      .then(() => console.log('Hub connection started'))
      .then(() => {
        this.hub.invoke('MonitorearBaterias', 1)
          .then(() => console.log('Joined BateriasGroup'))
          .catch(err => console.error('Error joining group:', err));
      })
      .catch(err => console.error('Error starting hub connection:', err));

    this.hub.on('MonitoreoBateria', (data: OPCBateria) => {

      try{

        var idx=data.tagsBateria[0].idTag-1;

        this.chartData[idx].push(...data.tagsBateria[0].mediciones.map(m => m.medicion));
        this.chartLabels[idx].push(...data.tagsBateria[0].mediciones.map(m => new Date(m.timestamp).toLocaleTimeString()));
        
        this.actualizarGrafica(idx);
      }catch(err){
        console.error('Error processing data:', err);
      }
      //}

    });
  }

  actualizarGrafica(idx: number) {
    const chartRefs = [
      this.chartRef1, this.chartRef2, this.chartRef3, this.chartRef4,
      this.chartRef5, this.chartRef6, this.chartRef7, this.chartRef8, this.chartRef9
    ];
    const chart = chartRefs[idx];
    if (chart && chart.chart) {
      chart.chart.data.labels = this.chartLabels[idx];
      chart.chart.data.datasets[0].data = this.chartData[idx];
      chart.chart.update();
    }
  }
}
