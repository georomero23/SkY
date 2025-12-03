
export class PanelModel {
    idPanel:number  = 0;

    idInstalacion:number  = 0;

    idEstadoPanel:number  = 0;

    numeroSerie:string  = "";

    modelo:string  = "";

    marca:string  = "";

    potencia:number  = 0;

    degradacionAnual:number  = 0;

    proveedorSuministrador:string  = "";

    proveedorSuministradorRfc:string  = "";

    proveedorIntermediario:string  = "";

    proveedorIntermediarioRfc:string  = "";

    esInicial:boolean  = false;

    danado:boolean  = false;

    numeroSerieReemplazo:string|null = null;
}

export class PanelSimple {
  public idPanel:number = 0;
  public numeroSerie:string = "";
  public modelo:string = "";
  public marca:string = "";
  public potencia:number = 0;
  public instalacion: string = "";
  public idInstalacion: number = 0;
  public idCliente: number = 0;
}
