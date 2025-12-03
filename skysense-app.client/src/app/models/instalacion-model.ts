import { ClienteModel } from "./cliente-model";
import { EstadoInstalacionModel } from "./estadoInstalacion-model";
import { PlataformaModel } from "./plataforma-model";

export class InstalacionModel {
    idInstalacion:number = 0;

    idCliente :number = 0;

    idPlataforma:number|null = null;
    zona:number|null = null;

    idCatalogoEstatus:number  |null = null;

    sEstatus: string = "";

    idCatalogoTarifa: number|null = null;

    sTarifa: string = "";

    nombre: string = "";

    tipoProyecto: number|null = null;

    codigoProyecto: string = "";

    longitud: string = "";

    latitud: string = "";

    estado: string = "";

    ubicacion: string = "";

    garantiaInicio:Date|undefined;

    garantiaFin:Date|undefined;

    idCatalogoGarantiaEstatus: number = 0;

    inicioOperaciones: Date | undefined;

    porcentajeDesgastePaneles: number | null = null;

    idInstalacionApi: string | null = null;

    rpu: string = "";

    idGrupo: number|null = null;

    anioInstalacion: number|null = null;

    contactoNombre: string | null = null;

    contactoTelefono: string | null = null;

    contactoEmail: string | null = null;

    idClienteNavigation: ClienteModel = new ClienteModel();

    potenciaInstalada:number|null = null;

    esFinanciado: boolean = false;

    grupoNombre: string = "";
}

export class GrupoModel {
    idGrupo: number = 0;
    nombre: string = "";
}
