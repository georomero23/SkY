import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Paneles'
    },
    children: [
      {
        path: '',
        redirectTo: 'paneles',
        pathMatch: 'full'
      },
      {
        path: 'paneles',
        loadComponent: () => import('./paneles.component').then(m => m.PanelesComponent),
        data: {
          title: 'Paneles'
        }
      }
    ]
  }
];

