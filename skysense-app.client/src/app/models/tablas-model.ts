export class Documento
{
    idDocumento : number = 0;
    idInstalacion: number = 0;

    tipoDocumento: number = 0;
    nombreDocumento : string = "";

    tamano : number = 0;
    tipoArchivo : string = "";

    fechaModificacion : Date|undefined;
}

export class ReciboMensual
{
    idRecibo: number = 0;
    idInstalacion: number = 0;

    mesRecibo: Date = new Date();

    idDocumento: number|null = null;

    kWhBase : number | null = null;

    kWhIntermedia : number | null = null;

    kWhPunta : number | null = null;

    kWbase : number | null = null;

    kWintermedia : number | null = null;

    kWpunta : number | null = null;

    reactivosKvArh : number | null = null;
    
    pago : number | null = null;

    idDocumentoNavigation: Documento | null = null;

    
}
export class ReporteMensual
{
    idReporte: number = 0;
    idInstalacion: number = 0;

    mesReporte: Date = new Date();

    idDocumento: number|null = null;

    idDocumentoNavigation: Documento | null = null;

    panelesGeneracion: number | null = null;
    ahorroAcumulado: number | null = null;
    ahorroAmbiental: number | null = null;
    consumoCFE: number | null = null;
    cargoDAP: number | null = null;
    umbralFP: number | null = null;
}

export class KPIInstalaciones{
     idInstalacion : number=0;
     nombre:string = "";
     idCliente :number=0;
     idGrupo:number|null = null;
     anno:number = 0;
     generacionReal:number|null = null;
     generacionGarantizada:number|null = null;
     porcentaje:number|null = null;
}