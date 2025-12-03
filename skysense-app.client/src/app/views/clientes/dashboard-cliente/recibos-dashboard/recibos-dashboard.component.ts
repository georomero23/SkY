import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonDirective, CardBodyComponent, CardComponent, CardHeaderComponent, FormModule, ModalBodyComponent, ModalModule, PageItemDirective, PageLinkDirective, PaginationComponent, SpinnerComponent, TableDirective, ToasterService, TooltipDirective } from '@coreui/angular';
import { Paginacion } from '@models/apiRespuestaModel';
import { Documento, ReciboMensual } from '@models/tablas-model';
import { NivelAlerta, ToastModel } from '@models/toast-model';
import { DashboardService } from '@services/InformacionEntrePantallas/dashboard.service';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { ToastService } from '@services/Front/toast.service';
import { ModalComponent } from "../../../../../../node_modules/@coreui/angular/lib/modal/modal/modal.component";
import { FileUploadComponent } from "src/app/components/cargaArchivos/file-upload.component";
import { IconDirective } from '@coreui/icons-angular';
import { iconSubset } from 'src/app/icons/icon-subset';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-recibos-dashboard',
  imports: [PaginationComponent, PageItemDirective, PageLinkDirective, CardComponent, SpinnerComponent, CardBodyComponent, 
    TooltipDirective, CardHeaderComponent, IconDirective, TableDirective, CommonModule, ModalModule, FileUploadComponent, 
    ButtonDirective, FormsModule],
  templateUrl: './recibos-dashboard.component.html',
  styleUrl: './recibos-dashboard.component.scss'
})
export class RecibosDashboardComponent {
tipoArchivo = 2; // 1 = recibos; 2 = recibos

iconos = iconSubset;

  _paginacion:Paginacion<Documento>|undefined;
  _paginasArray: number[] = [];

  #instalacionService:InstalacionesService = inject(InstalacionesService);
  #DashboardService:DashboardService = inject(DashboardService);
  #routerService:Router = inject(Router);
  #routeService:ActivatedRoute = inject(ActivatedRoute);
  #tostadaService:ToastService = inject(ToastService);

  _recibosAnuales: ReciboMensual[] = [];
  _anioSeleccionado: number = new Date().getFullYear();
  _subiendoMes: Date | null = null;

  isModalVisible = false;
  isPreviewVisible = false;
  previewUrl: SafeResourceUrl | null = null;
  previewType: 'pdf' | 'image' | 'other' = 'other';
  #sanitizer: DomSanitizer = inject(DomSanitizer);

  accion=0; // 0 = Ninguna, 1 = Cargar, 2 = Borrar, 3 = Descargar
  listaAnios: number[] = [];

  _reciboModificado: ReciboMensual | null = null;

constructor(){
      if(this.#DashboardService._Instalacion == undefined){
        this.#routerService.navigate(['.'], { relativeTo: this.#routeService.parent });
      }
    }

  ngOnInit(){
    this.obtenRecibos();
    this._anioSeleccionado = 2025
    // Por ejemplo, últimos 5 años
    const actual = new Date().getFullYear();
    this.listaAnios = Array.from({length: 5}, (_, i) => actual - i);
  }
  
  obtenRecibos(){
    this.#instalacionService.mObtenerRecibos(this.#DashboardService._Instalacion!.idInstalacion, this._anioSeleccionado).subscribe({
      next: (data) => {
        if (data.exito) {
          this._recibosAnuales = data.data;
          this._recibosAnuales.forEach(recibo => {
            recibo.mesRecibo = new Date(recibo.mesRecibo);
          });
        } else {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al obtener recibos", data.mensaje, 5, NivelAlerta.Advertencia));
        }
      },
      error: (err) => {
        this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al obtener recibos", "Ocurrió un error inesperado al obtener los recibos.", 5, NivelAlerta.Peligro));
      }
    });
  }

  cargarRecibos(file: File) {
    const doc = {
      idInstalacion: this.#DashboardService._Instalacion!.idInstalacion,
      tipoDocumento: this.tipoArchivo, // Recibo
      nombreDocumento: file.name,
      tipoArchivo: file.type,
      tamano: file.size
    };

    var formData = new FormData();
    formData.append("documentoJSON", JSON.stringify(doc));
    formData.append("archivo", file);
    formData.append("iAnno", this._subiendoMes?.toISOString()?.split('T')[0] ?? "null");

    this.accion=1; // Cargando
    this.#instalacionService.mModificaDocumento(formData).subscribe({
      next: (data)=>{
        if(data.exito !== true){
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al cargar el recibo", data.mensaje, 5, NivelAlerta.Advertencia));
        }else{
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Recibo cargado", "El recibo se cargó correctamente.", 5, NivelAlerta.Exito));
          this.obtenRecibos();
        }
      },
      error: (err)=>{
          this.accion=0; // Nada
          this.isModalVisible = false;
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al cargar el recibo", "Ocurrió un error inesperado al cargar el recibo.", 5, NivelAlerta.Peligro));
      },
      complete: ()=>{
        this.accion=0; // Nada
        this.isModalVisible = false;
      }
    });
  }

  borrarRecibo(_t21: Documento) {
    this.#instalacionService.mEliminaDocumento(_t21.idInstalacion, _t21.idDocumento!).subscribe({
      next: (data)=>{
        if(data.exito !== true){
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al eliminar el recibo", data.mensaje, 5, NivelAlerta.Advertencia));
        }else{
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Recibo eliminado", "El recibo se eliminó correctamente.", 5, NivelAlerta.Exito));
              this.obtenRecibos();
        }
      },
      error: (err)=>{
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al eliminar el recibo", "Ocurrió un error inesperado al eliminar el recibo.", 5, NivelAlerta.Peligro));
      },
      complete: ()=>{}
    });
  }

  descargarRecibo(_t21: Documento) {
    this.#instalacionService.descargarDocumento(_t21.idInstalacion, _t21.idDocumento!).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = _t21.nombreDocumento; // Usa el nombre original del recibo
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    });
  }

  visualizarRecibo(recibo: Documento) {
    this.#instalacionService.descargarDocumento(recibo.idInstalacion, recibo.idDocumento!).subscribe(blob => {
      const fileType = recibo.tipoArchivo;
      this.previewType = fileType.includes('pdf') ? 'pdf'
        : fileType.includes('image') ? 'image'
        : 'other';
      const url = URL.createObjectURL(blob);
      this.previewUrl = this.#sanitizer.bypassSecurityTrustResourceUrl(url);
      this.isPreviewVisible = true;
    });
  }

  editarRecibo(arg0: ReciboMensual) {
    this._reciboModificado = { ...arg0 };
  }

  mConfirmaEdicion(arg0: number) {
    this.#instalacionService.mModificaInfoRecibo(this._reciboModificado!).subscribe({
      next: (data) => {
        if (data.exito) {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Recibo modificado", "El recibo se modificó correctamente.", 5, NivelAlerta.Exito));
          this._reciboModificado = null;
          this.obtenRecibos();
        } else {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al modificar el recibo", data.mensaje, 5, NivelAlerta.Advertencia));
        }
      },
      error: (err) => {
        this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al modificar el recibo", "Ocurrió un error inesperado al modificar el recibo.", 5, NivelAlerta.Peligro));
      }
    });
  }
}
