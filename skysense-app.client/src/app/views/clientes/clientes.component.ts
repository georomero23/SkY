import { Component, inject, OnInit } from '@angular/core';
import { TextColorDirective, CardComponent, CardHeaderComponent, CardBodyComponent, FormControlDirective, FormDirective, FormLabelDirective, FormSelectDirective, ButtonDirective, TableDirective, SpinnerComponent, PaginationComponent, PageItemDirective, PageLinkDirective, ColComponent, CollapseDirective, InputGroupComponent, ModalModule, TooltipDirective } from '@coreui/angular';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { UserService } from '@services/Front/user.service';
import { ClientesService } from '@services/API/clientes-serivce';
import { ClienteModel } from '@models/cliente-model';
import { CommonModule } from '@angular/common';
import { IconDirective, IconSetService } from '@coreui/icons-angular';
import { RouterLink } from '@angular/router';
import { ToastService } from '@services/Front/toast.service';
import { Paginacion } from '@models/apiRespuestaModel';
import { InfoClienteFormComponent } from '../shared/info-cliente-form/info-cliente-form.component';
import { iconSubset } from 'src/app/icons/icon-subset';
import { ConfirmDialogComponent } from "src/app/components/confirm-dialog/confirm-dialog.component";
import { MiPaginadorComponent } from "src/app/components/mi-paginador/mi-paginador.component";

@Component({
  selector: 'app-clientes',
  imports: [SpinnerComponent, TextColorDirective, CardComponent, IconDirective, CardHeaderComponent, CardBodyComponent, FormControlDirective,
    ReactiveFormsModule, FormsModule, FormDirective, FormLabelDirective, CommonModule, ButtonDirective, TableDirective, PaginationComponent,
    PageItemDirective, PageLinkDirective, ColComponent, CollapseDirective, InputGroupComponent, ModalModule, InfoClienteFormComponent, TooltipDirective, ConfirmDialogComponent, MiPaginadorComponent],
  templateUrl: './clientes.component.html',
  styleUrl: './clientes.component.scss'
})
export class ClientesComponent implements OnInit {
  _clientesPaginados:Paginacion<ClienteModel> = new Paginacion<ClienteModel>();
  _busqueda = "";

  iconos = iconSubset;

  _modalNewClienteVisible = false;
  _modalNuevoCliente = false;

  #UserService:UserService = inject(UserService);
  #ClienteService:ClientesService = inject(ClientesService);
  #tostadaService:ToastService = inject(ToastService);


  _editandoRenglon: number = -1;
  _clienteModal:ClienteModel = new ClienteModel();


  filtroVisible = false;
  _modalConfirmacionVisible: boolean = false;

  constructor(){
    
  }

  ngOnInit(){
    
    this.ObtenerClientes(1,5);
  }

  ObtenerClientes(pagina:number, cuantos:number){
    this.#ClienteService.mObtenerClientes(pagina, cuantos, this._busqueda).subscribe(
        {
          next: value => {
            if(value.exito){
              this._clientesPaginados = value.data;
            }else{
              this.#tostadaService.mostrarError("Error al obtener clientes: " + value.mensaje);
            }
          },
          error: err => {
            this.#tostadaService.mostrarError("Ocurrió un error recuperando a los clientes.");
          }
        }
      );
  }

  

  paginacionCambiada(indexElegido:number){
    this._editandoRenglon = -1;
    this.ObtenerClientes(indexElegido,5);
  }

  mElimina(i:number){

  }

  mEdita(cliente: ClienteModel){
    this._clienteModal = Object.assign({}, cliente);
    this._modalNewClienteVisible = true;
    this._modalNuevoCliente = false;
  }

  BuscarCliente(){
    this._editandoRenglon=-1;
    this.ObtenerClientes(1, 5);
  }

  LimpiarBusqueda(){
    this._editandoRenglon=-1;
    this._busqueda="";
    this.ObtenerClientes(1, 5);
  }
  
  NuevoCliente() {
    this._clienteModal = new ClienteModel();
    this._modalNewClienteVisible = true;
    this._modalNuevoCliente = true;
  }

  GuardaInfoCliente(cliente: ClienteModel) {


    //Lógica para crear un nuevo cliente
    if (this._modalNuevoCliente) {
      this.#ClienteService.mNuevoCliente(cliente).subscribe(
        {
          next: value => {
            if (value.exito) {
              this.#tostadaService.mostrarExito("Cliente creado exitosamente.");
              this._modalNewClienteVisible = false;
              this.ObtenerClientes(1, 5);
            } else {
              this.#tostadaService.mostrarError("Error al crear cliente: " + value.mensaje);
            }
          },
          error: err => {
            this.#tostadaService.mostrarError("Ocurrió un error creando el cliente.");
          }
        }
      );
    } 
    // Lógica para editar un cliente existente
    else {
      this.#ClienteService.mModificaCliente(cliente).subscribe(
        {
          next: value => {
            if (value.exito) {
              this.#tostadaService.mostrarExito("Cliente editado exitosamente.");
              this._modalNewClienteVisible = false;
              this.ObtenerClientes(1, 5);
            } else {
              //Si ya existe, se pregunta la posibilidad de fusionar
              if(value.codigoError === 102){
                this._clienteModal = cliente;
                this._modalNewClienteVisible = false;
                //Abrir el diálogo de confirmación
                this._modalConfirmacionVisible = true;
              }else{
                this.#tostadaService.mostrarError("Error al editar cliente: " + value.mensaje);

              }
            }
          },
          error: err => {
            this.#tostadaService.mostrarError("Ocurrió un error editando el cliente.");
          }
        }
      );
    }
  }
  
  
  mFusiona($event:any) {
    this.#ClienteService.mFusionarClientes(this._clienteModal).subscribe(
      {
        next: value => {
          if (value.exito) {
            this.#tostadaService.mostrarExito("Clientes fusionados exitosamente: " + value.mensaje);
            this._modalNewClienteVisible = false;
            this._modalConfirmacionVisible = false;
            this.ObtenerClientes(1, 5);
          } else {
            this.#tostadaService.mostrarError("Error al fusionar clientes: " + value.mensaje);
          }
        },
        error: err => {
          this.#tostadaService.mostrarError("Ocurrió un error fusionando los clientes.");
            this._modalNewClienteVisible = false;
        }
      }
    );
  }
}
