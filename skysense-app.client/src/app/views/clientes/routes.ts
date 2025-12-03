import { Routes } from '@angular/router';
import { PendingChangesGuard } from 'src/app/guards/can-deactivate.guard';

export const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Clientes'
    },
    children: [
      {
        path: '',
        redirectTo: 'consulta',
        pathMatch: 'full'
      },
      {
        path: 'consulta',
        loadComponent: () => import('./clientes.component').then(m => m.ClientesComponent),
        //pathMatch: 'full',
        data: {
          title: 'Consulta'
        }
      },
      {
        path: ':idCliente',
        loadComponent: () => import('./dashboard-cliente/dashboard-cliente.component').then(m => m.DashboardClienteComponent),
        // pathMatch: 'full',
        children:[
          {
            path: '',
            redirectTo: '0',
            pathMatch: 'full',
            data: {
              title: 'Clientes'
            },
          },
          {
            path: ':idInstalacion',
            //redirectTo: ':idInstalacion/instalaciones',
            data: {
              title: ''
            },
            children: [
              {
                path: 'Instalaciones',
                loadComponent: ()=> import('./dashboard-cliente/instalacion-dashboard/instalacion-dashboard.component').then(m => m.InstalacionDashboardComponent)
                //,pathMatch: 'full'
              },
              {
                path: 'InstalacionInfo',
                canDeactivate: [PendingChangesGuard],
                loadComponent: ()=> import('./dashboard-cliente/info-instalacion/info-instalacion.component').then(m => m.InfoInstalacionComponent)
                //,pathMatch: 'full'
              }, 
              {
                path: 'Estadisticas',
                loadComponent: ()=> import('./dashboard-cliente/estadisticas-dashboard/estadisticas-dashboard.component').then(m => m.EstadisticasDashboardComponent),
                //pathMatch: 'full'
              },
              {
                path: 'Recibos',
                loadComponent: ()=> import('./dashboard-cliente/recibos-dashboard/recibos-dashboard.component').then(m => m.RecibosDashboardComponent),
                //pathMatch: 'full'
              },
              {
                path: 'Reportes',
                loadComponent: ()=> import('./dashboard-cliente/reportes-dashboard/reportes-dashboard.component').then(m => m.ReportesDashboardComponent),
                //pathMatch: 'full'
              },
              {
                path: 'Polizas',
                loadComponent: ()=> import('./dashboard-cliente/polizas-dashboard/polizas-dashboard.component').then(m => m.PolizasDashboardComponent),
                //pathMatch: 'full'
              },
              {
                path: 'Documentos',
                loadComponent: ()=> import('./dashboard-cliente/documentos/documentos.component').then(m => m.DocumentosComponent),
                //pathMatch: 'full'
              }
            ]
          }
        ]
      }
    ]
  }
];

