import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonDirective, CardBodyComponent, CardComponent, CardHeaderComponent, ModalBodyComponent, ModalModule, PageItemDirective, PageLinkDirective, PaginationComponent, SpinnerComponent, TableDirective, ToasterService, TooltipDirective } from '@coreui/angular';
import { Paginacion } from '@models/apiRespuestaModel';
import { Documento } from '@models/tablas-model';
import { NivelAlerta, ToastModel } from '@models/toast-model';
import { DashboardService } from '@services/InformacionEntrePantallas/dashboard.service';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { ToastService } from '@services/Front/toast.service';
import { ModalComponent } from "../../../../../../node_modules/@coreui/angular/lib/modal/modal/modal.component";
import { FileUploadComponent } from "src/app/components/cargaArchivos/file-upload.component";
import { IconDirective } from '@coreui/icons-angular';
import { iconSubset } from 'src/app/icons/icon-subset';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-recibos-dashboard',
  imports: [PaginationComponent, PageItemDirective, PageLinkDirective, CardComponent, SpinnerComponent, CardBodyComponent, TooltipDirective, CardHeaderComponent, IconDirective, TableDirective, CommonModule, ModalModule, FileUploadComponent, ButtonDirective],
  templateUrl: './polizas-dashboard.component.html',
  styleUrl: './polizas-dashboard.component.scss'
})
export class PolizasDashboardComponent implements OnInit {
  iconos = iconSubset;

  _paginacion:Paginacion<Documento>|undefined;
  _paginasArray: number[] = [];

  #instalacionService:InstalacionesService = inject(InstalacionesService);
  #DashboardService:DashboardService = inject(DashboardService);
  #routerService:Router = inject(Router);
  #routeService:ActivatedRoute = inject(ActivatedRoute);
  #tostadaService:ToastService = inject(ToastService);
  isModalVisible = false;
  isPreviewVisible = false;
  previewUrl: SafeResourceUrl | null = null;
  previewType: 'pdf' | 'image' | 'other' = 'other';
  #sanitizer: DomSanitizer = inject(DomSanitizer);

  accion=0; // 0 = Ninguna, 1 = Cargar, 2 = Borrar, 3 = Descargar

constructor(){
      if(this.#DashboardService._Instalacion == undefined){
        this.#routerService.navigate(['.'], { relativeTo: this.#routeService.parent });
      }
    }

  ngOnInit(){
    this.cambiaPagina(1);
  }

  cambiaPagina(pagina:number, forzar:boolean = false) {
    if(this._paginacion?.paginaActual === pagina && !forzar) return;
    const instalacion = this.#DashboardService._Instalacion!;
    this.#instalacionService.mObtenerDocumentos(instalacion.idInstalacion, pagina, 5, 2).subscribe({
      next: (data)=>{
        if(data.exito !== true){
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al obtener los datos", data.mensaje, 5, NivelAlerta.Advertencia));
        }else{
          this._paginacion = data.data;
          this._paginasArray = Array.from({ length: this._paginacion.numeroPaginas }, (_, i) => i + 1);
        }
      },
      error: (err)=>{
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al obtener los datos", "Ocurrió un error inesperado al obtener los datos.", 5, NivelAlerta.Peligro));
      },
      complete: ()=>{}
    })
  }

  cargarRecibos(file: File) {

    const Documento = {
      idInstalacion: this.#DashboardService._Instalacion!.idInstalacion,
      tipoDocumento: 2, // Recibo
      nombreDocumento: file.name,
      tipoArchivo: file.type,
      tamano: file.size
    };
    
    var formData = new FormData();
    formData.append("documentoJSON", JSON.stringify(Documento));
    formData.append("archivo", file);

    this.accion=1; // Cargando
    this.#instalacionService.mModificaDocumento(formData).subscribe({
      next: (data)=>{
        if(data.exito !== true){
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al cargar el recibo", data.mensaje, 5, NivelAlerta.Advertencia));
        }else{
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Recibo cargado", "El recibo se cargó correctamente.", 5, NivelAlerta.Exito));
          this.cambiaPagina(1,true);
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
          this.cambiaPagina(1,true);
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
}
