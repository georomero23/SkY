import { INavData } from '@coreui/angular';

export interface INavDataExtended extends INavData {
  roles?: string[]; // Array de roles permitidos
}

export const navItems: INavDataExtended[] = [
  // {
  //   name: 'Ir a',
  //   title: true
  // },
  {
    name: 'Administrador',
    iconComponent: { name: 'cil-dog' },
    url: '/admin',
    children: [
      {
        name: 'Usuarios',
        url: '/admin/users',
        iconComponent: { name: 'cil-people' }
      },
      {
        name: 'Catálogos',
        url: '/admin/catalog',
        iconComponent: { name: 'cil-list-rich' }
      },
      {
        name: 'Tarifas',
        url: '/admin/tarifas',
        iconComponent: { name: 'cil-dollar' },
        badge: {
          color: 'success',
          text: 'Nuevo'
        }
      },
      {
        name: 'Festivos',
        url: '/admin/festivos',
        iconComponent: { name: 'cil-beach-access' },
        badge: {
          color: 'success',
          text: 'Nuevo'
        }
      },
      {
        name: 'Baneo de correos',
        url: '/admin/banned',
        iconComponent: { name: 'cil-low-vision' },
        badge: {
          color: 'warning',
          text: 'Pronto'
        }
        
      }
    ],
    roles: ['Administrador']
  },
  {
    name: 'Instalaciones',
    url: '/instalaciones',
    iconComponent: { name: 'cil-factory' }
  },
  {
    name: 'Clientes',
    url: '/clientes/consulta',
    iconComponent: { name: 'cil-people' }
  },
  {
    name: 'Paneles',
    url: '/paneles',
    iconComponent: { name: 'cil-grid' }
  },
  {
    name: 'Inversores',
    iconComponent: { name: 'cil-sync' },
    url: '/inversores'
  },
  // {
  //   name: 'Baterias',
  //   iconComponent: { name: 'cil-battery-full' },
  //   url: '/baterias',
  //   children: [
  //     {
  //       name: 'Consulta',
  //       url: '/baterias/consulta',
  //       iconComponent: { name: 'cil-battery-3' }
  //     },
  //     {
  //       name: 'Monitoreo',
  //       url: '/baterias/monitoreo',
  //       iconComponent: { name: 'cil-graph' }
  //     },
  //     {
  //       name: 'Alertas',
  //       url: '/baterias/alertas',
  //       iconComponent: { name: 'cil-battery-alert' },
  //       badge: {
  //         color: 'warning',
  //         text: 'Pronto'
  //       }
  //     }
  //   ]
  // },
  // {
  //   name: 'Monitoreo',
  //   url: '/dashboard',
  //   iconComponent: { name: 'cil-factory' }
  // },
  // {
  //   name: 'Reporteo',
  //   url: '/dashboard',
  //   iconComponent: { name: 'cil-factory' }
  // },
  // {
  //   name: 'Cotizador de pólizas',
  //   url: '/dashboard',
  //   iconComponent: { name: 'cil-factory' }
  // },
  // {
  //   name: 'Comercial',
  //   title: true
  // },
  // {
  //   name: 'Cotización',
  //   url: '/dashboard',
  //   iconComponent: { name: 'cil-factory' }
  // },
  // {
  //   name: 'Cotizador',
  //   url: '/dashboard',
  //   iconComponent: { name: 'cil-factory' }
  // }
  // {
  //   name: 'Monitoreo',
  //   iconComponent: { name: 'cil-star' },
  //   url: '/icons',
  //   children: [
  //     {
  //       name: 'CoreUI Free',
  //       url: '/dashboard',
  //       icon: 'nav-icon-bullet',
  //       badge: {
  //         color: 'success',
  //         text: 'FREE'
  //       }
  //     },
  //     {
  //       name: 'CoreUI Flags',
  //       url: '/dashboard',
  //       icon: 'nav-icon-bullet'
  //     },
  //     {
  //       name: 'CoreUI Brands',
  //       url: '/icons/brands',
  //       icon: 'nav-icon-bullet'
  //     }
  //   ]
  // },
  // {
  //   name: 'Reporteo',
  //   url: '/notifications',
  //   iconComponent: { name: 'cil-bell' },
  //   children: [
  //     {
  //       name: 'Alerts',
  //       url: '/notifications/alerts',
  //       icon: 'nav-icon-bullet'
  //     },
  //     {
  //       name: 'Badges',
  //       url: '/notifications/badges',
  //       icon: 'nav-icon-bullet'
  //     },
  //     {
  //       name: 'Modal',
  //       url: '/notifications/modal',
  //       icon: 'nav-icon-bullet'
  //     },
  //     {
  //       name: 'Toast',
  //       url: '/notifications/toasts',
  //       icon: 'nav-icon-bullet'
  //     }
  //   ]
  // },
  // {
  //   name: 'Cotizador de pólizas',
  //   url: '/widgets',
  //   iconComponent: { name: 'cil-calculator' }
  //   // badge: {
  //   //   color: 'info',
  //   //   text: 'NEW'
  //   // }
  // },
  // {
  //   title: true,
  //   name: 'Comercial'
  // },
  // {
  //   name: 'Cotización',
  //   url: '/login',
  //   iconComponent: { name: 'cil-star' },
  //   children: [
  //     {
  //       name: 'Login',
  //       url: '/login',
  //       icon: 'nav-icon-bullet'
  //     },
  //     {
  //       name: 'Restablecer',
  //       url: '/restablecer',
  //       icon: 'nav-icon-bullet'
  //     },
  //     {
  //       name: 'Error 404',
  //       url: '/404',
  //       icon: 'nav-icon-bullet'
  //     },
  //     {
  //       name: 'Error 500',
  //       url: '/500',
  //       icon: 'nav-icon-bullet'
  //     }
  //   ]
  // },
  // {
  //   name: 'Cotizador',
  //   url: '/login',
  //   iconComponent: { name: 'cil-star' },
  //   children: [
  //     {
  //       name: 'Login',
  //       url: '/login',
  //       icon: 'nav-icon-bullet'
  //     },
  //     {
  //       name: 'Restablecer',
  //       url: '/restablecer',
  //       icon: 'nav-icon-bullet'
  //     },
  //     {
  //       name: 'Error 404',
  //       url: '/404',
  //       icon: 'nav-icon-bullet'
  //     },
  //     {
  //       name: 'Error 500',
  //       url: '/500',
  //       icon: 'nav-icon-bullet'
  //     }
  //   ]
  // }
  // {
  //   title: true,
  //   name: 'Links',
  //   class: 'mt-auto'
  // },
  // {
  //   name: 'Docs',
  //   url: 'https://coreui.io/angular/docs/',
  //   iconComponent: { name: 'cil-description' },
  //   attributes: { target: '_blank' }
  // }
];
