import { inject, Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class InterceptadorService implements HttpInterceptor {
  router = inject(Router);

  constructor() { }

  //Método para interceptar redireccionamientos del API
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
            if (error.status === 401) {
              this.router.navigate(['/login']);
            }
            return throwError(() => error);
          })
    );
  }
}
