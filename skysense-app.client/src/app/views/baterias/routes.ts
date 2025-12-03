import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Baterias'
    },
    children: [
      {
        path: '',
        redirectTo: 'monitoreo',
        pathMatch: 'full'
      },
      {
        path: 'consulta',
        loadComponent: () => import('./baterias-consulta/baterias-consulta.component').then(m => m.BateriasConsultaComponent),
        data: {
          title: 'Consulta'
        }
      },
      {
        path: 'alertas',
        loadComponent: () => import('./baterias-alertas/baterias-alertas.component').then(m => m.BateriasAlertasComponent),
        data: {
          title: 'Alertas'
        }
      },
      
      {
        path: 'monitoreo/:idInstalacion',
        loadComponent: () => import('./bateria-real-time/bateria-real-time.component').then(m => m.BateriaRealTimeComponent),
        data: {
          title: 'Batería en Tiempo Real'
        }
      },
      {
        path: 'monitoreo',
        loadComponent: () => import('./baterias-monitoreo/baterias-monitoreo.component').then(m => m.BateriasMonitoreoComponent),
        data: {
          title: 'Monitoreo'
        }
      }
    ]
  }
];

