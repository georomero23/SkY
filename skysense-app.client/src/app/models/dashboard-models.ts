import{ClienteModel} from './cliente-model'
import { PanelModel } from './panel-model';
import { InversorModel } from './inversor-model';

export class ClienteDashboard {
    cClienteInfo: ClienteModel | undefined;
    sImagenCliente: string ="";
    arrInstalaciones: InstalacionOpcion[] = [];
}

export class InstalacionOpcion{
    idCliente: number = 0;
    idInstalacion: number = 0;
    sNombreInstalacion: string = "";
    iTipoProyecto: number = 1;
}

export class InstalacionDashboard {
    idCliente: number = 0;
    idInstalacion: number = 0;
    sNumeroServicio: string = "";
    sTipoTarifa: string = "";
    sDireccion: string = "";
    cPanel: PanelModel| undefined;
    arrInversores: InversorModel[] = [];
    idInstalacionAPI: string = "";
    idPlataforma: number = 0;
    dtFechaInicioOperaciones!: Date;
    rPU: string = "";
}

export class TablaGeneracion {
    idInstalacion: number = 0;
    fPorcentajeDesgaste: number = 0;
    dtFechaInicioOperaciones!: Date;
    arrValoresGarantizados: InstalacionGeneracionMensual[] = [];
    arrValoresReales: (number | null)[] = [];
}

export class InstalacionGeneracionMensual {

    idInstalacion : number = 0;

    mes : number = 0;

    anno : number = 0;

    generacionGarantizada : number | null = null;

    daemonYaEjecutado : boolean = false;
}

export class TablaGeneracionDiaria {
    inversoresEncabezados: string[] = [];
    inversoresIdentificadores: string[] = [];
    datos: RenglonGeneracionDiaria[] = [];
}

export class RenglonGeneracionDiaria{
    iDia: number = 0;
    datosInversores: number[] = [];
    arregloEditados: boolean[]  = [];
    generacionTotal: number = 0;
    desviacionMaxima: number = 0;
}
