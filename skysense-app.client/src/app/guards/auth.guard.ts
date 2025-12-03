import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { inject, Injectable } from '@angular/core';
import { IdentityService } from '@services/API/identity.service';
import { Observable } from 'rxjs';
import { AuthService } from '@services/API/auth.service';
import { UserService } from '@services/Front/user.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  _userService = inject(UserService);
  _router = inject(Router);

  constructor() {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ):
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {

    const Roles = this._userService.Roles??[];
    if (this._userService.mEstoyLogueadoLocal()) {
      //Si la ruta tiene roles definidos Y si ningun rol es del usuario, se regresa a dashboard
      if (
        route.data['roles'] &&
        !Roles.some(
          (ur) => route.data['roles'].indexOf(ur) >= 0
        )
      ) {
        //console.log("aqui no entras mijo")
        return this._router.createUrlTree(['/dashboard']);
      //Si la ruta tiene roles no permitidos definidos Y si todos los roles no permitidos son del usuario
      // , se regresa a dashboard
      }else if (
        route.data['rolesNo'] &&
        Roles.every(
          (ur) => route.data['rolesNo'].indexOf(ur) >= 0
        )
      ) {
        return this._router.createUrlTree(['/dashboard']);
      }
    } else {
      // User is not logged in, redirect to login page
      return this._router.createUrlTree(['/login'], {
        queryParams: { returnUrl: state.url },
      });
    }

    return true;
  }
  canActivateChild(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ):
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    return this.canActivate(next, state);
  }
}
