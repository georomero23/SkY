
export class ClienteModel {
    idCliente: number = 0;
    nombre: string = "";
    rfc: string = "";
    correo: string = "";
    telefono: string = "";
    estado : number = 0;
    esEspecial: boolean = true;
    contacto: string = "";
    cuantasInstalaciones: number = 0;
}

export class RoleModel {
    idRol: string = "";
    name: string = "";
}
