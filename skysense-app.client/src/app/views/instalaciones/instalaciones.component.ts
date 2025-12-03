import { Component, inject, OnInit } from '@angular/core';
import { TextColorDirective, CardComponent, CardHeaderComponent, CardBodyComponent, FormControlDirective, FormDirective, FormLabelDirective, FormSelectDirective, ButtonDirective, TableDirective, SpinnerComponent, PaginationComponent, PageLinkDirective, PageItemDirective, InputGroupComponent, PlaceholderAnimationDirective, PlaceholderDirective, ColDirective } from '@coreui/angular';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { UserService } from '@services/Front/user.service';
import { InstalacionesService } from '@services/API/instalaciones-service';
import { GrupoModel, InstalacionModel } from '@models/instalacion-model';
import { CommonModule } from '@angular/common';
import { IconDirective } from '@coreui/icons-angular';
import { Paginacion } from '@models/apiRespuestaModel';
import { MiPaginadorComponent } from "src/app/components/mi-paginador/mi-paginador.component";


@Component({
  selector: 'app-instalaciones',
  imports: [SpinnerComponent, TextColorDirective, CardComponent, IconDirective,
    CardHeaderComponent, CardBodyComponent, FormControlDirective, ReactiveFormsModule, FormsModule, FormDirective, FormLabelDirective, CommonModule, ButtonDirective, TableDirective, PaginationComponent,
    PageItemDirective, PageLinkDirective, InputGroupComponent, PlaceholderAnimationDirective, PlaceholderDirective, ColDirective, MiPaginadorComponent],
  templateUrl: './instalaciones.component.html',
  styleUrl: './instalaciones.component.scss'
})
export class InstalacionesComponent {
  #InstalacionesService:InstalacionesService = inject(InstalacionesService);

  _error:boolean = false;

  _editandoRenglon: number = -1;
  _instalacionEditando!:InstalacionModel;
  _haciendoPeticion:boolean = false;
  _ordenandoPorNombreAscendente:boolean|undefined;
  
  _itemsPorPagina: number = 5;

  grupos: GrupoModel[] = [];
  grupoSeleccionado: string = "-1";

  _paginadoInstalaciones: Paginacion<InstalacionModel> = new Paginacion<InstalacionModel>();

  _cargando: boolean = false;

  _palabraBuscador:string|undefined;

  constructor(){
    
  }

  ngOnInit(){
    //console.log(this.#UserService.Usuario());
    this.ObtenerInstalaciones(1,this._itemsPorPagina);
    this.#InstalacionesService.mObtenerGrupos().subscribe({
      next: (data) => {
        if(data.exito){
          this.grupos = data.data;
        } else {
          this._error = true;
        }
      },
      error: () => { this._error = true; },
      complete: () => {}
    });
  }

  ObtenerInstalaciones(paginaActual:number, registrosPorPagina:number, grupo?:number){
    this._cargando = true;
    this.#InstalacionesService.mObtenerInstalaciones(paginaActual, registrosPorPagina, this._palabraBuscador?.toUpperCase(), grupo).subscribe(
        {
          next: value => {
            if(value.exito !== true){
              this._error = true;
            }else{
              this._paginadoInstalaciones = value.data;
            }
          },
          error: err => {this._error = true; this._cargando=false;}, // This will be called if catchError re-throws or doesn't handle the error
          complete: ()=>{
            this._cargando = false;
          }
        }
      );
  }

  mElimina(i:number){

  }

  mEdita(i:InstalacionModel){
    this._instalacionEditando = Object.assign({},i);
    this._editandoRenglon = i.idInstalacion;
  }

  mOrdenaPorNombre(){
    // if(!this._ordenandoPorNombreAscendente)
    //   this._ordenandoPorNombreAscendente = true;
    // else
    //   this._ordenandoPorNombreAscendente = !this._ordenandoPorNombreAscendente;

    // this.mInsertarInstalaciones(this._instalacionesPaginados.flat().sort((a,b)=> (this._ordenandoPorNombreAscendente?1:-1)*a.nombre.localeCompare(b.nombre)));
  }

    mConfirmaEdicion(i: number) {
      this._haciendoPeticion = true;
      this.#InstalacionesService.mModificaGrupo(this._instalacionEditando.idInstalacion, this._instalacionEditando.idGrupo??0).subscribe(
        {
          next: value => {if(value != null){
              this.ObtenerInstalaciones(this._paginadoInstalaciones.paginaActual,this._itemsPorPagina, +this.grupoSeleccionado);

              this._editandoRenglon = -1;
          } },
          error: err => {
            console.log(err);
            this._haciendoPeticion =false;
            this._editandoRenglon = -1; // This will be called if catchError re-throws or doesn't handle the error
          },
          complete: ()=>{
            this._haciendoPeticion =false;
          }
        }
      );
    }

  paginacionCambiada(indexElegido:number){
    this._editandoRenglon = -1;

    this.ObtenerInstalaciones(indexElegido, this._itemsPorPagina, +this.grupoSeleccionado);
  }

  BuscarInstalacion(){

    this._editandoRenglon=-1;
    
    this.ObtenerInstalaciones(1,this._itemsPorPagina, +this.grupoSeleccionado);
  }

  LimpiarFiltro(){
    
  }

  filtrarPorGrupo() {
    this.ObtenerInstalaciones(1,this._itemsPorPagina, +this.grupoSeleccionado);
  }

  
}
