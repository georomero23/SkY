import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'potenciaRedondeo'
})
export class PotenciaRedondeoPipe implements PipeTransform {

  transform(value: number, decimales: number, unidad: string ): string {
    let valor = value;
    let respuesta = "";
    if(valor >= 1000000)
    {
      valor = valor / 1000000;
      respuesta  = valor.toFixed(decimales) + " M"+unidad;
    }else if(valor > 1000){
      valor = valor / 1000;
      respuesta  = valor.toFixed(decimales) + " K"+unidad;
    } else{
      respuesta  = valor.toFixed(decimales) + " "+unidad;
    }

    return respuesta;
  }

}
