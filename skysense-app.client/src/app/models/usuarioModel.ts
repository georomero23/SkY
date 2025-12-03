
export class UsuarioModel {
    id!: string;
    name!: string;
    mail!: string;
    cntrsn: string|undefined;
    roles: string[] = []
}

export class UserTableModel{
    id:string ="";
    name: string ="";
    mail:string ="";
    bloqueado:boolean = false;
    modificaBloqueo :boolean  =false;
    rol:string="";
}
