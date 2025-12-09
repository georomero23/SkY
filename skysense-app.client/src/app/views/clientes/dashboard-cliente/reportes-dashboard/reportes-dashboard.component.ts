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

  tipoArchivo = 1;

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

  accion = 0;

  listaAnios: number[] = [];

  _reporteModificado: ReporteMensual | null = null;
  isAutoReportVisible: boolean = false;

  _idInstalacion: number = -1;

  _guardando: boolean = false;

  _reporteAutomaticoMes: number | null = null;
  _reporteAutomaticoAnno: number | null = null;

  private _datosReporteAuto: {
    panelesGeneracion: number;
    ahorroAcumulado: number;
    ahorroAmbiental: number;
    consumoCFE: number;
  } | undefined;

  configRA: ReporteAutomaticoConfig = {
    bajaTension2: false
  } as ReporteAutomaticoConfig;

  constructor() {
    if (this.#DashboardService._Instalacion == undefined) {
      this.#routerService.navigate(['Instalaciones'], { relativeTo: this.#routeService.parent });
    }
  }

  ngOnInit() {
    this.obtenReportes();
    this.obtenConfiguracionReportesAutomaticos();
    this._anioSeleccionado = 2025;

    const actual = new Date().getFullYear();
    this.listaAnios = Array.from({ length: 5 }, (_, i) => actual - i);
  }

  obtenReportes() {
    this._idInstalacion = this.#DashboardService._Instalacion!.idInstalacion;
    this.#instalacionService.mObtenerReportes(this._idInstalacion, this._anioSeleccionado).subscribe({
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
      error: () => {
        this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al obtener reportes", "Ocurrió un error inesperado al obtener los reportes.", 5, NivelAlerta.Peligro));
      }
    });
  }

  obtenConfiguracionReportesAutomaticos() {
    this.#instalacionService.mObtenConfigReporteAutomatico(this.#DashboardService._Instalacion!.idInstalacion).subscribe({
      next: (data) => {
        if (data.exito) {
          this.configRA = new ReporteAutomaticoConfig(data.data);
          this.configRA.porcentajeDapNumeros = this.configRA.porcentajeDap * 100;
          this.configRA.fpDefaultNumeros = this.configRA.fpDefault * 100;

        } else {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al obtener configuración", data.mensaje, 5, NivelAlerta.Advertencia));
        }
      },
      error: () => {
        this.#tostadaService.GeneraAlertaToast(new ToastModel("Error", "Error inesperado.", 5, NivelAlerta.Peligro));
      }
    });
  }

  private toBoolean(value: any): boolean {
    return value === true || value === 'true' || value === 1 || value === '1';
  }

  cargarReportes(file: File) {

    const Documento = {
      idInstalacion: this.#DashboardService._Instalacion!.idInstalacion,
      tipoDocumento: this.tipoArchivo,
      nombreDocumento: file.name,
      tipoArchivo: file.type,
      tamano: file.size
    };

    var formData = new FormData();
    formData.append("documentoJSON", JSON.stringify(Documento));
    formData.append("archivo", file);
    formData.append("iAnno", this._subiendoMes?.toISOString()?.split('T')[0] ?? "null");

    this.accion = 1;
    this.#instalacionService.mModificaDocumento(formData).subscribe({
      next: (data) => {
        if (!data.exito) {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al cargar reporte", data.mensaje, 5, NivelAlerta.Advertencia));
        } else {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Reporte cargado", "Cargado correctamente.", 5, NivelAlerta.Exito));
          this.obtenReportes();
        }
      },
      error: () => {
        this.accion = 0;
        this.isModalVisible = false;
        this.#tostadaService.GeneraAlertaToast(new ToastModel("Error", "Error inesperado.", 5, NivelAlerta.Peligro));
      },
      complete: () => {
        this.accion = 0;
        this.isModalVisible = false;
      }
    });
  }

  borrarReporte(_t21: Documento) {
    this.#instalacionService.mEliminaDocumento(_t21.idInstalacion, _t21.idDocumento!).subscribe({
      next: (data) => {
        if (!data.exito) {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error al eliminar", data.mensaje, 5, NivelAlerta.Advertencia));
        } else {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Reporte eliminado", "Se eliminó correctamente.", 5, NivelAlerta.Exito));
          this.obtenReportes();
        }
      },
      error: () => {
        this.#tostadaService.GeneraAlertaToast(new ToastModel("Error", "Error inesperado.", 5, NivelAlerta.Peligro));
      }
    });
  }

  descargarReporte(_t21: Documento) {
    this.#instalacionService.descargarDocumento(_t21.idInstalacion, _t21.idDocumento!).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = _t21.nombreDocumento;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
    });
  }

  visualizarReporte(reporte: Documento) {
    this.#instalacionService.descargarDocumento(reporte.idInstalacion, reporte.idDocumento!).subscribe(blob => {
      const fileType = reporte.tipoArchivo;
      this.previewType = fileType.includes('pdf')
        ? 'pdf'
        : fileType.includes('image')
          ? 'image'
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
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Reporte modificado", "Guardado.", 5, NivelAlerta.Exito));
          this._reporteModificado = null;
          this.obtenReportes();
        } else {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error", data.mensaje, 5, NivelAlerta.Advertencia));
        }
      },
      error: () => {
        this.#tostadaService.GeneraAlertaToast(new ToastModel("Error", "Error inesperado.", 5, NivelAlerta.Peligro));
      }
    });
  }

  generarReporteAutomatico(arg0: Date) {
    this._guardando = true;

    this._reporteAutomaticoMes = arg0.getMonth() + 1;
    this._reporteAutomaticoAnno = arg0.getFullYear(); 
    this.isAutoReportVisible = true;

    if (arg0.getMonth() == 0) {
      this.#instalacionService.mObtenerReportes(this.#DashboardService._Instalacion!.idInstalacion, this._anioSeleccionado - 1).subscribe({
        next: (data) => {
          if (data.exito) {
            data.data.forEach(reporte => {
              reporte.mesReporte = new Date(reporte.mesReporte.toString().replace(/-/g, '/'));
            });
          } else {
            this.#tostadaService.GeneraAlertaToast(new ToastModel("Error", data.mensaje, 5, NivelAlerta.Advertencia));
            this.isAutoReportVisible = false;
          }
        },
        error: () => {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error", "Error inesperado.", 5, NivelAlerta.Peligro));
          this.isAutoReportVisible = false;
        }
      });
    }
  }

  errorEnReporteHandler($event: string) {
    this.#tostadaService.GeneraAlertaToast(new ToastModel("Error", $event, 10, NivelAlerta.Peligro));
    this.isAutoReportVisible = false;
    this._reporteAutomaticoAnno = null;
    this._reporteAutomaticoMes = null;
  }

  exportarPDF() {

    this._guardando = true;

    const elementos = document.querySelectorAll('.reporte-automatico');
    const elemento = elementos[elementos.length - 1] as HTMLElement;

    if (!elemento) return;

    html2canvas(elemento, { scale: 2 }).then(canvas => {

      const imgData = canvas.toDataURL('image/jpg');
      const pdf = new jsPDF('p', 'mm', 'a4');
      const pageWidth = pdf.internal.pageSize.getWidth();

      const imgProps = pdf.getImageProperties(imgData);
      const pdfWidth = pageWidth;
      const pdfHeight = (imgProps.height * pdfWidth) / imgProps.width;

      pdf.addImage(imgData, 'JPEG', 0, 0, pdfWidth, pdfHeight);

      const nombreReporte = 'Reporte_' + (this._reporteAutomaticoMes?.toString().padStart(2, '0') ?? '00')
        + '-' + (this._reporteAutomaticoAnno ?? '0000') + '.pdf';

      pdf.save(nombreReporte);

      const pdfBase64 = pdf.output('datauristring').split(',')[1];

      const Documento = {
        idInstalacion: this.#DashboardService._Instalacion!.idInstalacion,
        tipoDocumento: this.tipoArchivo,
        nombreDocumento: nombreReporte,
        tipoArchivo: 'pdf'
      };

      var formData = new FormData();
      formData.append("documentoJSON", JSON.stringify(Documento));
      formData.append("archivo", pdfBase64);
      formData.append("Fecha", `${this._reporteAutomaticoAnno}-${(this._reporteAutomaticoMes ?? 0).toString().padStart(2, '0')}-01`);
      formData.append("panelGeneracion", this._datosReporteAuto?.panelesGeneracion?.toString() ?? "null");
      formData.append("ahorroAcumulado", this._datosReporteAuto?.ahorroAcumulado?.toString() ?? "null");
      formData.append("ahorroAmbiental", this._datosReporteAuto?.ahorroAmbiental?.toString() ?? "null");
      formData.append("consumoCFE", this._datosReporteAuto?.consumoCFE?.toString() ?? "null");

      formData.append("bajaTension2", this.configRA.bajaTension2 ? "true" : "false");

      this.#instalacionService.mGuardaReporteAutomatico(formData, this._idInstalacion).subscribe({
        next: (data) => {
          if (!data.exito) {
            this.#tostadaService.GeneraAlertaToast(new ToastModel("Error", data.mensaje, 5, NivelAlerta.Advertencia));
          } else {
            this.#tostadaService.GeneraAlertaToast(new ToastModel("Reporte cargado", "Correctamente.", 5, NivelAlerta.Exito));
            this.isAutoReportVisible = false;
            this.obtenReportes();
          }
        },
        error: () => {
          this.#tostadaService.GeneraAlertaToast(new ToastModel("Error", "Error inesperado.", 5, NivelAlerta.Peligro));
          this._guardando = false;
        },
        complete: () => {
          this._guardando = false;
        }
      });

    });

  }

  DatosReporteAutomaticoObtenidos($event: {
    panelesGeneracion: number;
    ahorroAcumulado: number;
    ahorroAmbiental: number;
    consumoCFE: number;
  }) {
    this._datosReporteAuto = { ...$event };
    this._guardando = false;
  }

  GuardaConfiguracionRA() {

    const nuevaConfig = new ReporteAutomaticoConfig();

    nuevaConfig.idInstalacion = this.configRA.idInstalacion;
    nuevaConfig.nombreEnRecibo = this.configRA.nombreEnRecibo;
    nuevaConfig.porcentajeDap = this.configRA.porcentajeDapNumeros / 100;
    nuevaConfig.umbralFp = this.configRA.umbralFp;
    nuevaConfig.fpDefault = this.configRA.fpDefaultNumeros / 100;
    nuevaConfig.bajaTension2 = this.configRA.bajaTension2;


    this.#instalacionService.mGuardaConfiguracionReporteAutomatico(this._idInstalacion, nuevaConfig)
      .subscribe({
        next: (data) => {
          if (!data.exito) {

            this.#tostadaService.GeneraAlertaToast(
              new ToastModel("Error", data.mensaje, 5, NivelAlerta.Advertencia)
            );

          } else {

            this.#tostadaService.GeneraAlertaToast(
              new ToastModel("Éxito", "Configuración guardada.", 5, NivelAlerta.Exito)
            );

            this.muestraConfig = false;

            this.obtenConfiguracionReportesAutomaticos();

          }
        },
        error: () => {
          this.#tostadaService.GeneraAlertaToast(
            new ToastModel("Error", "Error inesperado.", 5, NivelAlerta.Peligro)
          );
          this._guardando = false;
        },
        complete: () => {
          this._guardando = false;
        }
      });
  }
}
