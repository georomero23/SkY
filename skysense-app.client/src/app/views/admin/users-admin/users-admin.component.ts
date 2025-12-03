import { Component, inject, OnInit } from '@angular/core';
import { CardModule } from '@coreui/angular';
import { CatalogoEncabezadoModel } from '@models/catalogo-model';
import { UserTableModel } from '@models/usuarioModel';
import { InterfazService } from '@services/API/interfaz.service';
import { ToastService } from '@services/Front/toast.service';
import { MultiSelectComponent } from 'src/app/components/multi-select/multi-select.component';
import { RegistroEditorComponent } from 'src/app/components/registro-editor/registro-editor.component';

@Component({
  selector: 'app-users-admin',
  imports: [CardModule, RegistroEditorComponent, MultiSelectComponent],
  templateUrl: './users-admin.component.html',
  styleUrls: ['./users-admin.component.scss']
})
export class UsersAdminComponent implements OnInit {
  #tostadaService = inject(ToastService);
  #interfService = inject(InterfazService);

  usuarios: UserTableModel[] = [];
  seleccion: string[] = [];

  ngOnInit(): void {
    this.mObtenCatalogos();
    this.mObtenUsuarios();
  }

  mObtenCatalogos(){
    this.#interfService.obtenerOpcionesCatalogos(7).subscribe({
      next: (response) => {
        if (response.exito) {
          this.statusDataset = response.data.map((item: CatalogoEncabezadoModel) => ({ value: item.idCatalogo.toString(), label: item.nombre }));
        }else{
          this.#tostadaService.mostrarError(response.mensaje);
        }
      },
      error: (error) => {
        this.#tostadaService.mostrarError("Error al obtener opciones de catálogos");
      }
    });

    this.#interfService.obtenerRolesSistema().subscribe({
      next: (response) => {
        if (response.exito) {
          this.rolesDataset = response.data.map((item: string) => ({ value: item, label: item }));
        }else{
          this.#tostadaService.mostrarError(response.mensaje);
        }
      },
      error: (error) => {
        this.#tostadaService.mostrarError("Error al obtener roles del sistema");
      }
    });
  }

  mObtenUsuarios(){
    this.#interfService.obtenerUsuarios().subscribe({
      next: (response) => {
        if (response.exito) {
          this.usuarios = response.data;
        }else{
          this.#tostadaService.mostrarError(response.mensaje);
        }
      },
      error: (error) => {
        this.#tostadaService.mostrarError("Error al obtener usuarios");
      }
    });
  }

  // Dataset para selects
  rolesDataset : {value:string, label:string}[] = [];
  statusDataset : {value:string, label:string}[] = [];

  // Encabezados para el registro editor
  headers = [
    { key: 'name', label: 'Nombre' },
    { key: 'mail', label: 'Correo' },
    { key: 'roles', label: 'Roles', type: 'multiselect', options: this.rolesDataset },
    // { key: 'status', label: 'Status', type: 'select', options: this.statusDataset },
    // { key: 'activo', label: 'Activo', type: 'checkbox' }
  ];

  guardarUsuario(usuario: any) {
    this.#interfService.modificarUsuario(usuario).subscribe({
      next: (response) => {
        if (response.exito) {
          this.#tostadaService.mostrarExito("Usuario guardado exitosamente");
          this.mObtenUsuarios();
        } else {
          this.#tostadaService.mostrarError(response.mensaje);
        }
      },
      error: (error) => {
        this.#tostadaService.mostrarError("Error al guardar usuario");
      }
    });
  }

  eliminarUsuario(usuario: any) {
    // Lógica para eliminar usuario
  }
}
