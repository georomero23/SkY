import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Instalaciones'
    },
    children: [
      {
        path: '',
        redirectTo: 'instalaciones',
        pathMatch: 'full'
      },
      {
        path: 'instalaciones',
        loadComponent: () => import('./instalaciones.component').then(m => m.InstalacionesComponent),
        data: {
          title: 'Instalaciones'
        }
      }
    ]
  }
];

