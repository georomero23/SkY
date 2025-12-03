import { CommonModule } from '@angular/common';
import { Component, inject, Input, NgModule, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AccordionModule, ButtonDirective, FormCheckComponent, FormCheckInputDirective, FormCheckLabelDirective, FormControlDirective, InputGroupComponent, InputGroupTextDirective, TemplateIdDirective } from "@coreui/angular";
import { OPCBateria, TagBateria, TagSelectOptions } from '@models/baterias-models';
import { OpcionesSelect } from '@models/catalogo-model';
import { BateriasService } from '@services/API/baterias.service';
import { InterfazService } from '@services/API/interfaz.service';
import { ToastService } from '@services/Front/toast.service';
import { map, Observable, of } from 'rxjs';
import { RegistroEditorComponent } from "src/app/components/registro-editor/registro-editor.component";
import { ComponentCanDeactivate } from 'src/app/guards/can-deactivate.guard';
import { ConfirmDialogComponent } from "src/app/components/confirm-dialog/confirm-dialog.component";
import { YesNoDialogService } from '@services/Front/yes-no-dialog.service';
import { FilterPipe } from "../../../pipes/filter.pipe";

@Component({
  selector: 'app-info-conexiones-form',
  imports: [FormsModule, InputGroupComponent, RegistroEditorComponent, InputGroupTextDirective, FormControlDirective, AccordionModule, TemplateIdDirective,
    FormCheckComponent, FormCheckInputDirective, FormCheckLabelDirective, CommonModule, ButtonDirective, ConfirmDialogComponent, FilterPipe],
  templateUrl: './info-conexiones-form.component.html',
  styleUrl: './info-conexiones-form.component.scss'
})
export class InfoConexionesFormComponent implements OnInit, ComponentCanDeactivate {
  @Input() idInstalacion: number = 0;
  
  #dialogService = inject(YesNoDialogService);

  tostadaService = inject(ToastService);
  bateriasService = inject(BateriasService);
  
  tagsOpciones : TagSelectOptions[] = [];
  bateriaConfigOriginal:OPCBateria = new OPCBateria();
  bateriaConfigModificada:OPCBateria = { ...this.bateriaConfigOriginal };
  
  constructor(){}

  ngOnInit() {
    this.bateriasService.ObtenOpcionesTags().subscribe({
      next: (opciones)=>{
        if(opciones.exito){
          this.tagsOpciones = opciones.data;
        }else{
          this.tostadaService.mostrarError("Error al obtener las opciones de tags: " + opciones.mensaje);
        }
      },
      error: (err)=>{
        this.tostadaService.mostrarError("Error al obtener las opciones de tags.");
      }
    });

    this.bateriasService.ObtenConfiguracionBateria(this.idInstalacion).subscribe({
      next: (config)=>{
        if(config.exito){
          if(config.data == null){
            this.bateriaConfigOriginal = new OPCBateria();
            this.bateriaConfigOriginal.idInstalacion = this.idInstalacion;
          }else{
            this.bateriaConfigOriginal = config.data;
          }
          this.bateriaConfigModificada = { ...this.bateriaConfigOriginal, tagsBateria: [ ...this.bateriaConfigOriginal.tagsBateria ] };
        }else{
          this.tostadaService.mostrarError("Error al obtener la configuración de la batería: " + config.mensaje);
        }
      },
      error: (err)=>{
        this.tostadaService.mostrarError("Error al obtener la configuración de la batería: " + err);
      }
    });
  }

  GuardaConfiguracionBateria(){
    console.log(this.bateriaConfigModificada)
    this.bateriasService.GuardaConfiguracionBateria(this.bateriaConfigModificada).subscribe({
      next: (dato)=> {
        if(dato.exito){
          this.bateriaConfigOriginal = { ...this.bateriaConfigModificada, tagsBateria: [ ...this.bateriaConfigModificada.tagsBateria ] };
        }else{
          this.tostadaService.mostrarError("Error al guardar la configuración de la batería: " + dato.mensaje);
        }
      },
      error: (err)=>{
        this.tostadaService.mostrarError("Error al guardar la configuración de la batería.");
      }
    });
  }

    
  GuardarRegistro($event: any) {

    if($event.idTag && $event.idTag != 0){
      //Edición
      const index = this.bateriaConfigModificada.tagsBateria.findIndex(b=>b.idTag === $event.idTag);
      if(index !== -1){
        this.bateriaConfigModificada.tagsBateria[index] = {...$event};
      }
    }else{
      //Nuevo
      const tipo = this.tagsOpciones.find(t=>t.value == $event.idParametro)!;
      this.bateriaConfigModificada.tagsBateria.push({...$event, idTag: -(this.bateriaConfigModificada.tagsBateria.length + 1), 'graficable': tipo.graficable, 'soloLectura': tipo.soloLectura});
    }

    this.bateriaConfigModificada.tagsBateria = [ ...this.bateriaConfigModificada.tagsBateria ];
    console.log(this.bateriaConfigModificada.tagsBateria);
  }

  EliminaRegistro(index: number) {
    this.bateriaConfigModificada.tagsBateria = this.bateriaConfigModificada.tagsBateria.filter(b=>b.idTag !== index);
  }

  canDeactivate(){
    const original = JSON.stringify(this.bateriaConfigOriginal);
    const modificada = JSON.stringify(this.bateriaConfigModificada);

    if(original !== modificada){
      return this.#dialogService.open("Hay cambios sin guardar en la configuración de la batería. ¿Desea salir sin guardar?", "Cambios pendientes");
    }
    return of(true);
  }
}
