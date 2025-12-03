import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { AccordionModule, ButtonDirective, ModalModule, TemplateIdDirective } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { Documento } from '@models/tablas-model';
import { NivelAlerta, ToastModel } from '@models/toast-model';
import { DashboardService } from '@services/InformacionEntrePantallas/dashboard.service';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { ToastService } from '@services/Front/toast.service';
import { FileUploadComponent } from 'src/app/components/cargaArchivos/file-upload.component';
import { iconSubset } from 'src/app/icons/icon-subset';

@Component({
  selector: 'app-documentos',
  imports: [ AccordionModule, CommonModule, TemplateIdDirective, IconDirective, ModalModule, FileUploadComponent, ButtonDirective ],
  templateUrl: './documentos.component.html',
  styleUrl: './documentos.component.scss'
})
export class DocumentosComponent implements OnInit {

  iconos = iconSubset;
  #instalacionesService = inject(InstalacionesService);
  #tostadaService = inject(ToastService);
  #dashboardService = inject(DashboardService);
  #router = inject(Router);
  #routeService = inject(ActivatedRoute)
  _documentos: Documento[] = [];

  _subiendoTipo: number | null = null;

  isModalVisible = false;
  isPreviewVisible = false;
  previewUrl: SafeResourceUrl | null = null;
  previewType: 'pdf' | 'image' | 'other' = 'other';
  #sanitizer: DomSanitizer = inject(DomSanitizer);

  get documentosContratos() {
    return this._documentos.filter(d => d.tipoDocumento === 3);
  }
  
  get documentosPermisos() {
    return this._documentos.filter(d => d.tipoDocumento === 4);
  }
  
  get documentosVerificacion() {
    return this._documentos.filter(d => d.tipoDocumento === 5);
  }
  
  get documentosIngenieria() {
    return this._documentos.filter(d => d.tipoDocumento === 6);
  }
  
  get documentosOtros() {
    return this._documentos.filter(d => d.tipoDocumento === 7);
  }

  constructor(){
    if(this.#dashboardService._Instalacion == undefined){
      this.#router.navigate(['.'], { relativeTo: this.#routeService.parent });
    }

  }

  ngOnInit(){
    this.mCargaDocumentos();
  }

  mCargaDocumentos(){
    this.#instalacionesService.mObtenerDocumentosTipos(this.#dashboardService._Instalacion?.idInstalacion!, [3,4,5,6,7]).subscribe({
      next: (respuesta) => {
        if(respuesta.exito){
          this._documentos = respuesta.data;
        }else{
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al cargar documentos", respuesta.mensaje, 5, NivelAlerta.Advertencia));
        }
      },
      error: (error) => {
        this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al cargar documentos", "Ocurrió un error inesperado al cargar los documentos.", 5, NivelAlerta.Peligro));
      }
    });

  }

  cargarDocumento(file: File) {

    const Documento = {
      idInstalacion: this.#dashboardService._Instalacion!.idInstalacion,
      tipoDocumento: this._subiendoTipo, // Documento Tipo
      nombreDocumento: file.name,
      tipoArchivo: file.type,
      tamano: file.size
    };
    
    var formData = new FormData();
    formData.append("documentoJSON", JSON.stringify(Documento));
    formData.append("archivo", file);

   // this.accion=1; // Cargando
    this.#instalacionesService.mModificaDocumento(formData).subscribe({
      next: (data)=>{
        if(data.exito !== true){
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al cargar el documento", data.mensaje, 5, NivelAlerta.Advertencia));
        }else{
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Documento cargado", "El documento se cargó correctamente.", 5, NivelAlerta.Exito));
          this.mCargaDocumentos();
        }
      },
      error: (err)=>{
          //this.accion=0; // Nada
          this.isModalVisible = false;
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al cargar el documento", "Ocurrió un error inesperado al cargar el documento.", 5, NivelAlerta.Peligro));
      },
      complete: ()=>{
        //this.accion=0; // Nada
        this.isModalVisible = false;
      }
    });
  }

  borrarDocumento(_t21: Documento) {
    this.#instalacionesService.mEliminaDocumento(_t21.idInstalacion, _t21.idDocumento!).subscribe({
      next: (data)=>{
        if(data.exito !== true){
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al eliminar el documento", data.mensaje, 5, NivelAlerta.Advertencia));
        }else{
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Documento eliminado", "El documento se eliminó correctamente.", 5, NivelAlerta.Exito));
          this.mCargaDocumentos();
        }
      },
      error: (err)=>{
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al eliminar el documento", "Ocurrió un error inesperado al eliminar el documento.", 5, NivelAlerta.Peligro));
      },
      complete: ()=>{}
    });
  }

  descargarDocumento(_t21: Documento) {
    this.#instalacionesService.descargarDocumento(_t21.idInstalacion, _t21.idDocumento!).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = _t21.nombreDocumento; // Usa el nombre original del documento
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    });
  }

  visualizarDocumento(documento: Documento) {
    this.#instalacionesService.descargarDocumento(documento.idInstalacion, documento.idDocumento!).subscribe(blob => {
      const fileType = documento.tipoArchivo;
      this.previewType = fileType.includes('pdf') ? 'pdf'
        : fileType.includes('image') ? 'image'
        : 'other';
      const url = URL.createObjectURL(blob);
      this.previewUrl = this.#sanitizer.bypassSecurityTrustResourceUrl(url);
      this.isPreviewVisible = true;
    });
  }

}
