import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonDirective, CardBodyComponent, CardComponent, CardHeaderComponent, FormLabelDirective, FormModule, FormSelectDirective, InputGroupComponent, InputGroupTextDirective, ModalModule, PageItemDirective, PageLinkDirective, PaginationComponent, SpinnerComponent, TableDirective, ToasterService, TooltipDirective } from '@coreui/angular';
import { Paginacion } from '@models/apiRespuestaModel';
import { Documento, ReporteMensual } from '@models/tablas-model';
import { NivelAlerta, ToastModel } from '@models/toast-model';
import { DashboardService } from '@services/InformacionEntrePantallas/dashboard.service';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { ToastService } from '@services/Front/toast.service';
import { FileUploadComponent } from "src/app/components/cargaArchivos/file-upload.component";
import { IconDirective } from '@coreui/icons-angular';
import { iconSubset } from 'src/app/icons/icon-subset';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { ReporteAutomaticoComponent } from "src/app/components/reporte-automatico/reporte-automatico.component";
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';
import { ReporteAutomaticoConfig } from '@models/reporteAutomatico-model';


@Component({
  selector: 'app-reportes-dashboard',
  imports: [PaginationComponent, PageItemDirective, PageLinkDirective, CardComponent, SpinnerComponent, CardBodyComponent,
    TooltipDirective, CardHeaderComponent, IconDirective, TableDirective, CommonModule, ModalModule, FileUploadComponent,
    ButtonDirective, FormsModule, ReporteAutomaticoComponent, FormLabelDirective, FormModule, FormSelectDirective, InputGroupComponent, InputGroupTextDirective],
  templateUrl: './reportes-dashboard.component.html',
  styleUrl: './reportes-dashboard.component.scss'
})
export class ReportesDashboardComponent {
  muestraConfig: boolean = false;

  tipoArchivo = 1; // 1 = reportes; 2 = recibos

  iconos = iconSubset;

  _paginacion: Paginacion<Documento> | undefined;
  _paginasArray: number[] = [];

  #instalacionService: InstalacionesService = inject(InstalacionesService);
  #DashboardService: DashboardService = inject(DashboardService);
  #routerService: Router = inject(Router);
  #routeService: ActivatedRoute = inject(ActivatedRoute);
  #tostadaService: ToastService = inject(ToastService);

  _reportesAnuales: ReporteMensual[] = [];
  _anioSeleccionado: number = new Date().getFullYear();
  _subiendoMes: Date | null = null;

  isModalVisible = false;
  isPreviewVisible = false;
  previewUrl: SafeResourceUrl | null = null;
  previewType: 'pdf' | 'image' | 'other' = 'other';
  #sanitizer: DomSanitizer = inject(DomSanitizer);

  accion = 0; // 0 = Ninguna, 1 = Cargar, 2 = Borrar, 3 = Descargar
  listaAnios: number[] = [];

  _reporteModificado: ReporteMensual | null = null;
  isAutoReportVisible: boolean = false;

  _idInstalacion: number = -1;

  _guardando: boolean = false;

  _reporteAutomaticoMes: number | undefined;
  _reporteAutomaticoAnno: number | undefined;
  private _datosReporteAuto: { panelesGeneracion: number; ahorroAcumulado: number; ahorroAmbiental: number; consumoCFE: number; } | undefined;

  configRA: ReporteAutomaticoConfig = new ReporteAutomaticoConfig();

  constructor() {
    if (this.#DashboardService._Instalacion == undefined) {
      this.#routerService.navigate(['Instalaciones'], { relativeTo: this.#routeService.parent });
    }
  }

  ngOnInit() {
    this.obtenReportes();
    this.obtenConfiguracionReportesAutomaticos();
    this._anioSeleccionado = 2025
    // Por ejemplo, últimos 5 años
    const actual = new Date().getFullYear();
    this.listaAnios = Array.from({ length: 5 }, (_, i) => actual - i);
  }

  obtenReportes() {
    this._idInstalacion = this.#DashboardService._Instalacion!.idInstalacion;
    this.#instalacionService.mObtenerReportes(this.#DashboardService._Instalacion!.idInstalacion, this._anioSeleccionado).subscribe({
      next: (data) => {
        if (data.exito) {
          this._reportesAnuales = data.data;
          this._reportesAnuales.forEach(reporte => {
            reporte.mesReporte = new Date(reporte.mesReporte.toString().replace(/-/g, '/'));
          });
        } else {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al obtener reportes", data.mensaje, 5, NivelAlerta.Advertencia));
        }
      },
      error: (err) => {
        this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al obtener reportes", "Ocurrió un error inesperado al obtener los reportes.", 5, NivelAlerta.Peligro));
      }
    });
  }

  obtenConfiguracionReportesAutomaticos(){
    this.#instalacionService.mObtenConfigReporteAutomatico(this.#DashboardService._Instalacion!.idInstalacion).subscribe({
      next: (data) => {
        if (data.exito) {
          this.configRA = data.data;
          this.configRA.porcentajeDapNumeros = this.configRA.porcentajeDap*100;
          this.configRA.fpDefaultNumeros = this.configRA.fpDefault*100;
        } else {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al obtener configuración de reportes automáticos", data.mensaje, 5, NivelAlerta.Advertencia));
        }
      },
      error: (err) => {
        this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al obtener configuración de reportes automáticos", "Ocurrió un error inesperado al obtener la configuración.", 5, NivelAlerta.Peligro));
        console.log(err);
      }
    });
  }

  cargarReportes(file: File) {
    const Documento = {
      idInstalacion: this.#DashboardService._Instalacion!.idInstalacion,
      tipoDocumento: this.tipoArchivo, // Reporte
      nombreDocumento: file.name,
      tipoArchivo: file.type,
      tamano: file.size
    };

    var formData = new FormData();
    formData.append("documentoJSON", JSON.stringify(Documento));
    formData.append("archivo", file);
    formData.append("iAnno", this._subiendoMes?.toISOString()?.split('T')[0] ?? "null");

    this.accion = 1; // Cargando
    this.#instalacionService.mModificaDocumento(formData).subscribe({
      next: (data) => {
        if (data.exito !== true) {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al cargar el reporte", data.mensaje, 5, NivelAlerta.Advertencia));
        } else {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Reporte cargado", "El reporte se cargó correctamente.", 5, NivelAlerta.Exito));
          this.obtenReportes();
        }
      },
      error: (err) => {
        this.accion = 0; // Nada
        this.isModalVisible = false;
        this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al cargar el reporte", "Ocurrió un error inesperado al cargar el reporte.", 5, NivelAlerta.Peligro));
      },
      complete: () => {
        this.accion = 0; // Nada
        this.isModalVisible = false;
      }
    });
  }

  borrarReporte(_t21: Documento) {
    this.#instalacionService.mEliminaDocumento(_t21.idInstalacion, _t21.idDocumento!).subscribe({
      next: (data) => {
        if (data.exito !== true) {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al eliminar el reporte", data.mensaje, 5, NivelAlerta.Advertencia));
        } else {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Reporte eliminado", "El reporte se eliminó correctamente.", 5, NivelAlerta.Exito));
          this.obtenReportes();
        }
      },
      error: (err) => {
        this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al eliminar el reporte", "Ocurrió un error inesperado al eliminar el reporte.", 5, NivelAlerta.Peligro));
      },
      complete: () => { }
    });
  }

  descargarReporte(_t21: Documento) {
    this.#instalacionService.descargarDocumento(_t21.idInstalacion, _t21.idDocumento!).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = _t21.nombreDocumento; // Usa el nombre original del reporte
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    });
  }

  visualizarReporte(reporte: Documento) {
    this.#instalacionService.descargarDocumento(reporte.idInstalacion, reporte.idDocumento!).subscribe(blob => {
      const fileType = reporte.tipoArchivo;
      this.previewType = fileType.includes('pdf') ? 'pdf'
        : fileType.includes('image') ? 'image'
          : 'other';
      const url = URL.createObjectURL(blob);
      this.previewUrl = this.#sanitizer.bypassSecurityTrustResourceUrl(url);
      this.isPreviewVisible = true;
    });
  }

  editarReporte(arg0: ReporteMensual) {
    this._reporteModificado = { ...arg0 };
  }

  mConfirmaEdicion(arg0: number) {
    this.#instalacionService.mModificaInfoReporte(this._reporteModificado!).subscribe({
      next: (data) => {
        if (data.exito) {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Reporte modificado", "El reporte se modificó correctamente.", 5, NivelAlerta.Exito));
          this._reporteModificado = null;
          this.obtenReportes();
        } else {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al modificar el reporte", data.mensaje, 5, NivelAlerta.Advertencia));
        }
      },
      error: (err) => {
        this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al modificar el reporte", "Ocurrió un error inesperado al modificar el reporte.", 5, NivelAlerta.Peligro));
      }
    });
  }

  generarReporteAutomatico(arg0: Date) {
    //Se muestra el modal, se insertan los datos necesarios para el reporte automatico en variables para el componente
    this._guardando = true;
    this._reporteAutomaticoMes = arg0.getMonth() + 1;
    this._reporteAutomaticoAnno = arg0.getFullYear();
    this.isAutoReportVisible = true;

    //Si es enero, tomamos el reporte de Diciembre del año pasado para los sugeridos de DAP y Umbral FP
    if(arg0.getMonth() == 0){
      this._idInstalacion = this.#DashboardService._Instalacion!.idInstalacion;
      this.#instalacionService.mObtenerReportes(this.#DashboardService._Instalacion!.idInstalacion, this._anioSeleccionado-1).subscribe({
        next: (data) => {
          if (data.exito) {
            data.data.forEach(reporte => {
              reporte.mesReporte = new Date(reporte.mesReporte.toString().replace(/-/g, '/'));
            });
          } else {
            this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al obtener reportes", data.mensaje, 5, NivelAlerta.Advertencia));
            this.isAutoReportVisible = false;
          }
        },
        error: (err) => {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al obtener reportes", "Ocurrió un error inesperado al obtener los reportes.", 5, NivelAlerta.Peligro));
          this.isAutoReportVisible = false;
        }
      });
    }
  }


  errorEnReporteHandler($event: string) {
    this.#tostadaService.GeneraAlertaToast(new ToastModel("Error en reporte automático", $event, 10, NivelAlerta.Peligro));
    this.isAutoReportVisible = false;
    this._reporteAutomaticoAnno = undefined;
    this._reporteAutomaticoMes = undefined;
  }

  exportarPDF() {

    this._guardando = true;

    const elemento = document.querySelector('.reporte-automatico') as HTMLElement;
    if (!elemento) return;

    html2canvas(elemento, { scale: 2 }).then(canvas => {
      const imgData = canvas.toDataURL('image/jpg');
      const pdf = new jsPDF('p', 'mm', 'a4');
      const pageWidth = pdf.internal.pageSize.getWidth();
      const pageHeight = pdf.internal.pageSize.getHeight();

      // Ajusta el tamaño de la imagen al ancho de la página
      const imgProps = pdf.getImageProperties(imgData);
      const pdfWidth = pageWidth;
      const pdfHeight = (imgProps.height * pdfWidth) / imgProps.width;

      pdf.addImage(imgData, 'JPEG', 0, 0, pdfWidth, pdfHeight, 'alias', 'FAST');

      const nombreReporte = 'Reporte_' + (this._reporteAutomaticoMes?.toString().padStart(2, '0') ?? '00') + '-' + (this._reporteAutomaticoAnno ?? '0000') + '.pdf';
      // Guarda localmente
      pdf.save(nombreReporte);

      // Envía al servidor como base64
      const pdfBase64 = pdf.output('datauristring').split(',')[1];
      const Documento = {
        idInstalacion: this.#DashboardService._Instalacion!.idInstalacion,
        tipoDocumento: this.tipoArchivo, // Reporte
        nombreDocumento: nombreReporte,
        tipoArchivo: 'pdf'
      };

      var formData = new FormData();
      formData.append("documentoJSON", JSON.stringify(Documento));
      formData.append("archivo", pdfBase64);
      formData.append("Fecha", (this._reporteAutomaticoAnno?.toString() ?? "null") + "-" + (this._reporteAutomaticoMes?.toString()?.padStart(2, '0')??0) + "-01");
      formData.append("panelGeneracion", this._datosReporteAuto?.panelesGeneracion?.toString() ?? "null");
      formData.append("ahorroAcumulado", this._datosReporteAuto?.ahorroAcumulado?.toString() ?? "null");
      formData.append("ahorroAmbiental", this._datosReporteAuto?.ahorroAmbiental?.toString() ?? "null");
      formData.append("consumoCFE", this._datosReporteAuto?.consumoCFE?.toString() ?? "null");



      this.#instalacionService.mGuardaReporteAutomatico(formData, this._idInstalacion).subscribe({
        next: (data) => {
          if (data.exito !== true) {
            this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al cargar el reporte", data.mensaje, 5, NivelAlerta.Advertencia));
          } else {
            this.#tostadaService.GeneraAlertaToast(new ToastModel("Reporte cargado", "El reporte se cargó correctamente.", 5, NivelAlerta.Exito));
            this.isAutoReportVisible = false;
            this.obtenReportes();
          }
        },
        error: (err) => {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al cargar el reporte", "Ocurrió un error inesperado al cargar el reporte.", 5, NivelAlerta.Peligro));
          this._guardando = false;
        },
        complete: ()=>{
          this._guardando = false;

        }
      });
    }, );
  }

  DatosReporteAutomaticoObtenidos($event: { panelesGeneracion: number; ahorroAcumulado: number; ahorroAmbiental: number; consumoCFE: number; }) {
    this._datosReporteAuto = {...$event};
    this._guardando = false;
  }

  GuardaConfiguracionRA() {
    const nuevaConfig = {...this.configRA};
    nuevaConfig.porcentajeDap = nuevaConfig.porcentajeDapNumeros / 100;
    nuevaConfig.fpDefault = nuevaConfig.fpDefaultNumeros / 100;

    this.#instalacionService.mGuardaConfiguracionReporteAutomatico(this._idInstalacion, nuevaConfig).subscribe({
        next: (data) => {
          if (data.exito !== true) {
            this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al guardar la configuración", data.mensaje, 5, NivelAlerta.Advertencia));
          } else {
            this.#tostadaService.GeneraAlertaToast(new ToastModel("Éxito", "La configuración fue guardada correctamente.", 5, NivelAlerta.Exito));
            this.muestraConfig = false;
            this.configRA = nuevaConfig;
          }
        },
        error: (err) => {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al guardar", "Ocurrió un error inesperado al guardar la configuración.", 5, NivelAlerta.Peligro));
          this._guardando = false;
        },
        complete: ()=>{
          this._guardando = false;

        }
      });
  }
}
