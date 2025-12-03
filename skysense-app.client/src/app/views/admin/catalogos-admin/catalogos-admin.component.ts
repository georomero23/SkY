import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { N } from '@angular/core/navigation_types.d-u4EOrrdZ';
import { FormsModule } from '@angular/forms';
import { CardModule, TabDirective, TabPaneComponent, Tabs2Module, TabsModule } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { ApiRespuestaModel } from '@models/apiRespuestaModel';
import { CatalogoEncabezadoModel, CatalogoModel } from '@models/catalogo-model';
import { GrupoModel } from '@models/instalacion-model';
import { InterfazService } from '@services/API/interfaz.service';
import { ToastService } from '@services/Front/toast.service';
import { first, Observable, Subject } from 'rxjs';
import { RegistroEditorComponent } from 'src/app/components/registro-editor/registro-editor.component';
import { iconSubset } from 'src/app/icons/icon-subset';
import { ConfirmDialogComponent } from "src/app/components/confirm-dialog/confirm-dialog.component";

@Component({
  selector: 'app-catalogos-admin',
  imports: [RegistroEditorComponent, CardModule, FormsModule, CommonModule, Tabs2Module, IconDirective, TabDirective, ConfirmDialogComponent],
  templateUrl: './catalogos-admin.component.html',
  styleUrls: ['./catalogos-admin.component.scss']
})
export class CatalogosAdminComponent implements OnInit {
  #tostadaService = inject(ToastService);
  #userInterfaceService = inject(InterfazService);
  iconos = iconSubset;
  // #tostadaService = inject(ToastService);
  // #tostadaService = inject(ToastService);

  _tabSelected = 'Cat';

  catalogos: CatalogoEncabezadoModel[] = [];

  registros: CatalogoModel[] = [];
  grupos: GrupoModel[] = [];

  catalogoSeleccionado: number = -1;

  showConfirm: boolean = false;
  private confirmSubject = new Subject<boolean>();

  constructor() {}

  ngOnInit() {
    // Aquí puedes realizar la lógica de inicialización que necesites
    this.getCatalogos();
    this.getGrupos();
  }

  getCatalogos(){
     return this.#userInterfaceService.obtenerCatalogosMaestros().subscribe({
      next: (response) => {
        if (response.exito) {
          this.catalogos = response.data;
        } else {
          this.#tostadaService.mostrarError(response.mensaje);
        }
      },
      error: (error) => {
        this.#tostadaService.mostrarError("Error al obtener catálogos");
      }
    });
  }

  getHeaders() {
    return [
      { key: 'nombreOpcion', label: 'Opción', type: 'text' },
      { key: 'infoAdicional', label: 'Descripción', type: 'text' },
      { key: 'disponible', label: 'Disponible', type: 'checkbox' }
    ];
  }

  getRegistros(key: number) {
    this.#userInterfaceService.obtenerOpcionesCatalogos(key)
    .subscribe({
      next: (response) => {
        if (response.exito) {
          this.registros = response.data[0].opciones;
        } else {
          this.#tostadaService.mostrarError(response.mensaje);
        }
      },
      error: (error) => {
        this.#tostadaService.mostrarError("Error al obtener registros");
      }
    });
  }

  getGrupos() {
    this.#userInterfaceService.obtenerGrupos().subscribe({
      next: (response) => {
        if (response.exito) {
          this.grupos = response.data;
        } else {
          this.#tostadaService.mostrarError(response.mensaje);
        }
      },
      error: (error) => {
        this.#tostadaService.mostrarError("Error al obtener registros");
      }
    });
  }

  guardarCatalogo(registro: any) {

    registro.idCatalogo = this.catalogoSeleccionado;
    this.#userInterfaceService.guardarCatalogo(registro).subscribe({
      next: (response) => {
        if (response.exito) {
          this.#tostadaService.GeneraAlertaToastDirecta("Éxito", "Registro guardado correctamente");
          this.getRegistros(this.catalogoSeleccionado);
        } else {
          this.#tostadaService.GeneraAlertaToastDirecta("Error", response.mensaje);
        }
      },
      error: (error) => {
        this.#tostadaService.GeneraAlertaToastDirecta("Error", "Error al guardar el registro");
      }
    }); 
  }

  eliminarCatalogo(registro: any) {
    var cat = new CatalogoModel();
    cat.idOpcion = registro;
    cat.idCatalogo = this.catalogoSeleccionado;

    this.#userInterfaceService.eliminarCatalogo(cat).subscribe({
      next: (response) => {
        if (response.exito) {
          this.#tostadaService.GeneraAlertaToastDirecta("Éxito", "Registro eliminado correctamente");
          this.getRegistros(this.catalogoSeleccionado);
        } else {
          this.#tostadaService.GeneraAlertaToastDirecta("Error", response.mensaje);
        }
      },
      error: (error) => {
        this.#tostadaService.GeneraAlertaToastDirecta("Error", "Error al eliminar el registro");
      }
    });
  }


  
  confirmEliminarGrupo($event: any) {

    this.confirmSubject.pipe(first()).subscribe(confirmado => {
      if (confirmado) {
        this.#userInterfaceService.eliminarGrupo($event).subscribe({
          next: (response) => {
            if (response.exito) {
              this.#tostadaService.GeneraAlertaToastDirecta("Éxito", "Registro eliminado correctamente");
              this.getGrupos();
            } else {
              this.#tostadaService.GeneraAlertaToastDirecta("Error", response.mensaje);
            }
          },
          error: (error) => {
            this.#tostadaService.GeneraAlertaToastDirecta("Error", "Error al eliminar el registro");
          }
        });
      }
    });

    this.showConfirm = true;
  }

  guardarGrupo($event: any) {
    this.#userInterfaceService.guardarGrupo($event).subscribe({
      next: (response) => {
        if (response.exito) {
          this.#tostadaService.GeneraAlertaToastDirecta("Éxito", "Registro guardado correctamente");
          this.getGrupos();
        } else {
          this.#tostadaService.GeneraAlertaToastDirecta("Error", response.mensaje);
        }
      },
      error: (error) => {
        this.#tostadaService.GeneraAlertaToastDirecta("Error", "Error al guardar el registro");
      }
    }); 
  }


  
  eliminarGrupoYES() {
    this.confirmSubject.next(true);
    this.showConfirm = false;
  }

  eliminarGrupoNO() {
    this.confirmSubject.next(false);
    this.showConfirm = false;
  }
}
