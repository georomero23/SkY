import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Inversores'
    },
    children: [
      {
        path: '',
        redirectTo: 'inversores',
        pathMatch: 'full'
      },
      {
        path: 'inversores',
        loadComponent: () => import('./inversores.component').then(m => m.InversoresComponent),
        data: {
          title: 'Inversores'
        }
      }
    ]
  }
];

