import { Component, inject, OnInit } from '@angular/core';
import { TextColorDirective, CardComponent, CardHeaderComponent, CardBodyComponent, FormControlDirective, FormDirective, FormLabelDirective, FormSelectDirective, ButtonDirective, TableDirective, SpinnerComponent, PaginationModule, InputGroupComponent } from '@coreui/angular';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { UserService } from '@services/Front/user.service';
import { InversoresService } from '@services/API/inversores-service';
import { InversorModel, InversorSimple } from '@models/inversor-model';
import { CommonModule } from '@angular/common';
import { IconDirective, IconSetService } from '@coreui/icons-angular';
import { Paginacion } from '@models/apiRespuestaModel';
import { ToastService } from '@services/Front/toast.service';
import { NivelAlerta } from '@models/toast-model';
import { RouterLink } from '@angular/router';
import { MiPaginadorComponent } from "src/app/components/mi-paginador/mi-paginador.component";

 
@Component({
  selector: 'app-inversores',
  imports: [SpinnerComponent, TextColorDirective, CardComponent, IconDirective, CardHeaderComponent, CardBodyComponent,
    FormControlDirective, ReactiveFormsModule, FormsModule, FormDirective, FormLabelDirective, CommonModule, ButtonDirective,
    TableDirective, PaginationModule, RouterLink, InputGroupComponent, MiPaginadorComponent],
  templateUrl: './inversores.component.html',
  styleUrl: './inversores.component.scss'
})
export class InversoresComponent {
  _inversoresPaginados:Paginacion<InversorSimple> = new Paginacion<InversorSimple>();
  _paginasArray:number[] = [];

  buscador: string | undefined;

  #UserService:UserService = inject(UserService);
  #InversoresService:InversoresService = inject(InversoresService);
  #tostadaService = inject(ToastService);

  _error:boolean = false;

_editandoRenglon: number = -1;
_inversorEditando!:InversorModel;
_haciendoPeticion:boolean = false;
_ordenandoPorNombreAscendente:boolean|undefined;

  constructor(){
    
  }

  ngOnInit(){
    //console.log(this.#UserService.Usuario());
    this.ObtenerInversores(1);
  }

  ObtenerInversores(pagina: number = 1){

    this.#InversoresService.mObtenerInversoresPaginados(pagina,10,this.buscador?.toUpperCase()).subscribe({
      next: (valor)=>{
        if(valor.exito){
          this._inversoresPaginados = valor.data;
          this._paginasArray = Array.from({length: valor.data.numeroPaginas}, (_, i) => i + 1);
          //this._haciendoPeticion = false;
        }else{
          this.#tostadaService.GeneraAlertaToastDirecta("Error al obtener inversores", valor.mensaje, NivelAlerta.Advertencia);
        }
      },
      error: (error)=>{
        this.#tostadaService.GeneraAlertaToastDirecta("Error al obtener inversores", "Ocurrió un error inesperado al procesar la solicitud.", NivelAlerta.Peligro);
      },
      complete: ()=>{
        this._haciendoPeticion = false;
      }
    });
  }

  // mOrdenaPorNombre(){
  //   if(!this._ordenandoPorNombreAscendente)
  //     this._ordenandoPorNombreAscendente = true;
  //   else
  //     this._ordenandoPorNombreAscendente = !this._ordenandoPorNombreAscendente;

  //   this._inversores.sort((a,b)=> (this._ordenandoPorNombreAscendente?1:-1)*a.marca.localeCompare(b.marca));
  // }

}
