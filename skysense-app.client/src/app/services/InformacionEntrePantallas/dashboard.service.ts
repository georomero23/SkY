import { Injectable } from '@angular/core';
import { InstalacionDashboard } from '@models/dashboard-models';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  _Instalacion: InstalacionDashboard | undefined;
  constructor() { }
}
