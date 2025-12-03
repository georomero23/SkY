
export class InversorModel {
    public idInversor:number = 0;

    public idInstalacion:number = 0;

    public idEstadoInversor:number = 0;

    public numeroSerie:string = "";

    public modelo:string = "";

    public marca:string = "";

    public potencia:number = 0;

    public degradacionAnual:number = 0;

    public proveedorSuministrador:string = "";

    public proveedorSuministradorRfc:string = "";

    public proveedorIntermediario:string = "";

    public proveedorIntermediarioRfc:string = "";

    public esInicial:boolean = false;

    public danado:boolean = false;

    public numeroSerieReemplazo:string | null = null;
}

export class InversorSimple {
  public idInversor:number = 0;
  public numeroSerie:string = "";
  public modelo:string = "";
  public marca:string = "";
  public potencia:number = 0;
  public instalacion: string = "";
  public idInstalacion: number = 0;
  public idCliente: number = 0;
}

export class InversorDatosAjustados {
  idInstalacion: number = 0;
  identificadorInversor: string = "";
  fechaRegistro: Date|string = new Date();
  valorOriginal: number = 0;
  valorNuevo: number = 0;
}

export class InversorGeneracionDiaria{
  idInversorApi:number = 0;

  idConsecutivo:number = 0;

  fechaValor: Date = new Date();
   fechaUltimaActualizacion: Date = new Date();

  valor: number = 0;

   valorManual: boolean = false;
}

