import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Administrador'
    },
    children: [
      {
        path: '',
        redirectTo: 'buttons',
        pathMatch: 'full'
      },
      {
        path: 'users',
        loadComponent: () => import('./users-admin/users-admin.component').then(m => m.UsersAdminComponent),
        data: {
          title: 'Usuarios'
        }
      },
      {
        path: 'catalog', 
        loadComponent: () => import('./catalogos-admin/catalogos-admin.component').then(m => m.CatalogosAdminComponent),
        data: {
          title: 'Catálogos'
        }
      },
      {
        path: 'tarifas', 
        loadComponent: () => import('./zonas-tarifas/zonas-tarifas.component').then(m => m.ZonasTarifasComponent),
        data: {
          title: 'Tarifas'
        }
      },
      {
        path: 'festivos', 
        loadComponent: () => import('./dias-inhabiles/dias-inhabiles.component').then(m => m.DiasInhabilesComponent),
        data: {
          title: 'Festivos'
        }
      },
      {
        path: 'banning',
        redirectTo: '',
        //loadComponent: () => import('./banning/banning.component').then(m => m.BanningComponent),
        data: {
          title: 'Baneo de correos'
        }
      },
    ]
  }
];

