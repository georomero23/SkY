import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Usuario'
    },
    children: [
      {
        path: '',
        redirectTo: 'perfil',
        pathMatch: 'full'
      },
      {
        path: 'perfil',
        loadComponent: () => import('./profile/profile.component').then(m => m.ProfileComponent),
        data: {
          title: 'Perfil'
        }
      }
    ]
  }
];

