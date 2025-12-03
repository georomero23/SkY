import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';

export interface YesNoDialogData {
  message: string;
  title: string;
  response$: Subject<boolean>;
}

@Injectable({ providedIn: 'root' })
export class YesNoDialogService {
  private dialogSubject = new Subject<YesNoDialogData>();
  dialog$: Observable<YesNoDialogData> = this.dialogSubject.asObservable();

  open(message: string, title: string = 'Confirmación'): Observable<boolean> {
    const response$ = new Subject<boolean>();
    this.dialogSubject.next({ message, title, response$ });
    return response$.asObservable();
  }
}
