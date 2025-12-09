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
        loadComponent: () =>
          import('./dashboard-cliente/dashboard-cliente.component')
            .then(m => m.DashboardClienteComponent),
        children: [
          {
            path: '',
            redirectTo: 'Instalaciones',
            pathMatch: 'full'
          },
          {
            path: ':idInstalacion',
            children: [
              {
                path: '',
                redirectTo: 'Instalaciones',
                pathMatch: 'full'
              },
              {
                path: 'Instalaciones',
                loadComponent: () =>
                  import('./dashboard-cliente/instalacion-dashboard/instalacion-dashboard.component')
                    .then(m => m.InstalacionDashboardComponent),
              },
              {
                path: 'InstalacionInfo',
                canDeactivate: [PendingChangesGuard],
                loadComponent: () =>
                  import('./dashboard-cliente/info-instalacion/info-instalacion.component')
                    .then(m => m.InfoInstalacionComponent),
              },
              {
                path: 'Estadisticas',
                loadComponent: () =>
                  import('./dashboard-cliente/estadisticas-dashboard/estadisticas-dashboard.component')
                    .then(m => m.EstadisticasDashboardComponent),
              },
              {
                path: 'Recibos',
                loadComponent: () =>
                  import('./dashboard-cliente/recibos-dashboard/recibos-dashboard.component')
                    .then(m => m.RecibosDashboardComponent),
              },
              {
                path: 'Reportes',
                loadComponent: () =>
                  import('./dashboard-cliente/reportes-dashboard/reportes-dashboard.component')
                    .then(m => m.ReportesDashboardComponent),
              },
              {
                path: 'Polizas',
                loadComponent: () =>
                  import('./dashboard-cliente/polizas-dashboard/polizas-dashboard.component')
                    .then(m => m.PolizasDashboardComponent),
              },
              {
                path: 'Documentos',
                loadComponent: () =>
                  import('./dashboard-cliente/documentos/documentos.component')
                    .then(m => m.DocumentosComponent),
              }
            ]
          }
        ]
      }
    ]
  }
];
