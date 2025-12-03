import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { NgScrollbar } from 'ngx-scrollbar';

import { IconDirective } from '@coreui/icons-angular';
import {
  ContainerComponent,
  ShadowOnScrollDirective,
  SidebarBrandComponent,
  SidebarComponent,
  SidebarFooterComponent,
  SidebarHeaderComponent,
  SidebarNavComponent,
  SidebarToggleDirective,
  SidebarTogglerDirective
} from '@coreui/angular';

import { DefaultFooterComponent, DefaultHeaderComponent } from './';
import { INavDataExtended, navItems } from './_nav';
import { UserService } from '@services/Front/user.service';
import { IdentityService } from '@services/API/identity.service';
import { AuthService } from '@services/API/auth.service';

function isOverflown(element: HTMLElement) {
  return (
    element.scrollHeight > element.clientHeight ||
    element.scrollWidth > element.clientWidth
  );
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html',
  styleUrls: ['./default-layout.component.scss'],
  imports: [
    SidebarComponent,
    SidebarHeaderComponent,
    SidebarBrandComponent,
    SidebarNavComponent,
    SidebarFooterComponent,
    SidebarToggleDirective,
    SidebarTogglerDirective,
    ContainerComponent,
    DefaultHeaderComponent,
    IconDirective,
    NgScrollbar,
    RouterOutlet,
    RouterLink,
    ShadowOnScrollDirective
  ]
})
export class DefaultLayoutComponent implements OnInit {
  public navItems = [...navItems];
  _userService = inject(UserService);
  _authService = inject(AuthService);

  ngOnInit(): void {

    this.navItems = this.filteredNavItems;

    this._authService.mObtenInfoUsuarioLogueado().subscribe({
      next: (value)=>{
        if(value.exito){
          this._userService.mLoguearLocal(value.data);
        }else{
          this._userService.Desconectar();
        }
      },
      error: (err)=>{},
      complete: ()=>{}
    });

    
  }

  get filteredNavItems() {
  // Recursivo para filtrar hijos también
  const filterItems = (items: INavDataExtended[]): INavDataExtended[] => items
    .filter(item => !item.roles || item.roles.includes(this._userService.Roles?.[0]??""))
    .map(item => ({
      ...item,
      children: item.children ? filterItems(item.children) : undefined
    }));
  return filterItems(navItems);
}

}
