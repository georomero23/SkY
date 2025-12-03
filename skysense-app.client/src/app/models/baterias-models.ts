    export class OPCBateria
    {
        idInstalacion:number = 0;
        idBateria:number = 0
        monitorear: boolean = false;
        nombreInstalacion:string = '';
        url:string = '';
        tagsBateria:TagBateria[] = [];
    }

    export class TagBateria
    {
        idTag:number = 0;
        idParametro:number = 0;
        nombreAMostrar:string = '';
        mediciones:TagMedicion[] = [];
        etiqueta:string = '';
        unidad:string = '';
        monitorear:boolean = false;
        graficable:boolean = false;
        soloLectura:boolean = true;
        idGrafica?:number;
    }

    export class TagMedicion
    {
        idTag:number = 0;
        medicion:number = 0;
        timestamp:Date = new Date();
    }

    export class TagSelectOptions{
        value:string = '';
        label:string = '';
        graficable:boolean = false;
        soloLectura:boolean = true;
    }

    export class BateriaTabla{
        ultimaActualizacion: Date = new Date();
        baterias: BateriaRegistro[] = [];
    }

    export class BateriaRegistro{
        idInstalacion:number = 0;
        estatus:string = '';
        idEstatus:number = 0;
        nombreInstalacion:string = '';
        alertas:number = 0;
        tagsTotales: number = 0;
        tagsOnline: number = 0;
        mensajeUltimo: string = "-";
    }