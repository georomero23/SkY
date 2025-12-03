import { Component, DestroyRef, Inject, inject, OnInit, viewChild } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { delay, filter, map, tap } from 'rxjs/operators';

import { ColorModeService, ToastComponent, ToasterComponent, ToasterPlacement } from '@coreui/angular';
import { IconSetService } from '@coreui/icons-angular';
import { iconSubset } from './icons/icon-subset';
import { UserService } from '@services/Front/user.service'
import { IdentityService } from '@services/API/identity.service'
import { ToastService } from '@services/Front/toast.service';
import { NivelAlerta, ToastModel } from '@models/toast-model';
import { TostadaComponent } from './components/tostada/tostada.component';
import { YesNoDialogComponent } from "./components/dialog/dialog.component";

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    imports: [RouterOutlet, ToasterComponent, YesNoDialogComponent]
})
export class AppComponent implements OnInit {
  _userService = inject(UserService);
  _identityService = inject(IdentityService);
  _toastService = inject(ToastService);
  placement = ToasterPlacement.TopEnd;
  readonly toaster = viewChild(ToasterComponent);
  
  title = 'Skysense Portal';

  readonly #destroyRef: DestroyRef = inject(DestroyRef);
  readonly #activatedRoute: ActivatedRoute = inject(ActivatedRoute);
  readonly #router = inject(Router);
  readonly #routerActv = inject(ActivatedRoute);
  readonly #titleService = inject(Title);

  readonly #colorModeService = inject(ColorModeService);
  readonly #iconSetService = inject(IconSetService);

  constructor() {
    this._toastService.toastrSubject$.subscribe({
      next: (n)=> this.CrearAlertaToast((n as ToastModel))
    });

    this.#titleService.setTitle(this.title);
    // iconSet singleton
    this.#iconSetService.icons = { ...iconSubset };
    this.#colorModeService.localStorageItemName.set('coreui-free-angular-admin-template-theme-default');
    this.#colorModeService.eventName.set('ColorSchemeChange');
  }

  ngOnInit(): void {
    this.#router.events.pipe(
        takeUntilDestroyed(this.#destroyRef)
      ).subscribe((evt) => {
      if (!(evt instanceof NavigationEnd)) {
        return;
      }
    });

    this.#activatedRoute.queryParams
      .pipe(
        delay(1),
        map(params => <string>params['theme']?.match(/^[A-Za-z0-9\s]+/)?.[0]),
        filter(theme => ['dark', 'light', 'auto'].includes(theme)),
        tap(theme => {
          this.#colorModeService.colorMode.set(theme);
        }),
        takeUntilDestroyed(this.#destroyRef)
      )
      .subscribe();
  }

  CrearAlertaToast(toast: ToastModel){
    const options = {
      title: toast.Encabezado,
      mensaje: toast.Texto,
      delay: toast.Tiempo*1000,//en segundos
      placement: this.placement,
      color: toast.Nivel == NivelAlerta.Exito? 'success':toast.Nivel == NivelAlerta.Advertencia?'warning':toast.Nivel == NivelAlerta.Peligro?'danger':'info',
      autohide: true
    };
    const componentRef = this.toaster()?.addToast(TostadaComponent, { ...options });
  }
}
