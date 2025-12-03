
export class CatalogoEncabezadoModel{
    idCatalogo: number = 0;
    nombre: string = "";
    infoAdicional: string = "";
    disponible: boolean = false;
    opciones: CatalogoModel[] = [];
}

export class CatalogoModel {
    idCatalogo: number = 0;
    idOpcion: number = 0;
    nombreOpcion: string = "";
    infoAdicional: string = "";
    disponible: boolean = false;
}

export class OpcionesSelect{
    value: string = "";
    label: string = "";
}

export class ZonaTarifaModel{
    idTarifa :number = 0;

    idDivision:number = 0;

    anno:number = 0;

    mes:number = 0;

    valorTransmision:number | null = null;

    valorDistribucion:number | null = null;

    valorOpCenace:number | null = null;

    valorOpSsb:number | null = null;

    valorServiciosNoMem:number | null = null;

    valorEnergiaBase:number | null = null;

    valorEnergiaIntermedia:number | null = null;

    valorEnergiaPunta:number | null = null;

    valorEnergiaSemipunta:number | null = null;

    valorCapacidad:number | null = null;
}
