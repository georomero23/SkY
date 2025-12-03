import { Routes } from '@angular/router';
import { DefaultLayoutComponent } from './layout';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: '',
    component: DefaultLayoutComponent,
    canActivate: [AuthGuard],
    canActivateChild: [AuthGuard],
    data: {
      title: 'Home'
    },
    children: [
      {
        path: 'dashboard',
        loadChildren: () => import('./views/dashboard/routes').then((m) => m.routes)
      },
      {
        path: 'admin',
        loadChildren: () => import('./views/admin/routes').then((m) => m.routes),
        canActivate: [AuthGuard],
        data:{
          roles: ["Administrador"]
        }
      },
      {
        path: 'clientes',
        loadChildren: () => import('./views/clientes/routes').then((m) => m.routes),
        canActivate: [AuthGuard],
        data:{
          rolesNo: ["Cliente", "Operador Nivel 03"]
        }
      },
      {
        path: 'inversores',
        loadChildren: () => import('./views/inversores/routes').then((m) => m.routes),
        canActivate: [AuthGuard],
        data:{
          rolesNo: ["Cliente", "Operador Nivel 03"]
        }
      },
      {
        path: 'paneles',
        loadChildren: () => import('./views/paneles/routes').then((m) => m.routes),
        canActivate: [AuthGuard],
        data:{
          rolesNo: ["Cliente", "Operador Nivel 03"]
        }
      },
      {
        path: 'instalaciones',
        loadChildren: () => import('./views/instalaciones/routes').then((m) => m.routes),
        canActivate: [AuthGuard],
        data:{
          rolesNo: ["Cliente", "Operador Nivel 03"]
        }
      },
      {
        path: 'baterias',
        loadChildren: () => import('./views/baterias/routes').then((m) => m.routes),
        canActivate: [AuthGuard],
        data:{
          rolesNo: ["Cliente", "Operador Nivel 03"]
        }
      },
      // {
      //   path: 'forms',
      //   loadChildren: () => import('./views/forms/routes').then((m) => m.routes)
      // },
      // {
      //   path: 'icons',
      //   loadChildren: () => import('./views/icons/routes').then((m) => m.routes)
      // },
      // {
      //   path: 'notifications',
      //   loadChildren: () => import('./views/notifications/routes').then((m) => m.routes)
      // },
      // {
      //   path: 'widgets',
      //   loadChildren: () => import('./views/widgets/routes').then((m) => m.routes)
      // },
      // {
      //   path: 'charts',
      //   loadChildren: () => import('./views/charts/routes').then((m) => m.routes)
      // }
    ]
  },
  {
    path: '404',
    loadComponent: () => import('./views/pages/page404/page404.component').then(m => m.Page404Component),
    data: {
      title: 'Page 404'
    }
  },
  {
    path: '500',
    loadComponent: () => import('./views/pages/page500/page500.component').then(m => m.Page500Component),
    data: {
      title: 'Page 500'
    }
  },
  {
    path: 'login',
    loadComponent: () => import('./views/pages/login/login.component').then(m => m.LoginComponent),
    data: {
      title: 'Login Page'
    }
  },
  {
    path: 'restablecer',
    loadComponent: () => import('./views/pages/restablecer/restablecer.component').then(m => m.RestablecerComponent),
    data: {
      title: 'Restablecer contraseña'
    }
  },
  {
    path: 'registrar',
    loadComponent: () => import('./views/pages/register/register.component').then(m => m.RegisterComponent),
    data: {
      title: 'Registro de usuario'
    }
  },
  { path: '**', redirectTo: 'dashboard' }
];
