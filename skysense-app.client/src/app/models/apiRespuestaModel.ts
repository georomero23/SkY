
export class ApiRespuestaModel<T> {
    exito!: boolean;
    mensaje!: string;
    codigoError!: number;

    data!: T;
}

export class Paginacion<T>
{
    totalRegistros : number = 0;
    registrosPorPagina : number = 0;
    numeroPaginas: number = 0;
    paginaActual : number = 0;
    masDatos : boolean = false;
    registros: T[] = [];
}


