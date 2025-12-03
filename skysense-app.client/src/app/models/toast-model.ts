
export class ToastModel {

    constructor(Encabezado: string|undefined,Texto: string, Tiempo?: number, Nivel?: NivelAlerta ){
        this.Encabezado = Encabezado;
        this.Texto = Texto;
        this.Tiempo = Tiempo??5;
        this.Nivel = Nivel??NivelAlerta.Informacion;
    }

    Encabezado: string|undefined;
    Texto: string = "";
    Tiempo: number = 10;
    Nivel: NivelAlerta = 0;
}

export enum NivelAlerta{
    Informacion,
    Exito,
    Advertencia,
    Peligro
} 
