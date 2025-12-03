import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { HubConnectionBuilder } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  controllerName:string = "skyhub"
  baseURL:string;

  private http = inject(HttpClient);
  constructor() {
      this.baseURL=this.controllerName;
   }

   InicializaSignalR(){
    return new HubConnectionBuilder()
      .withUrl('/api/skyhub')
      .withAutomaticReconnect()
      .build();
   }


   

}
