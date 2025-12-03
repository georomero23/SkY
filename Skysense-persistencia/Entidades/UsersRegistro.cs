using System;
using System.Collections.Generic;

namespace Skysense_persistencia.Entidades;

public partial class UsersRegistro
{
    public int IdRegistro { get; set; }

    public string? Nombre { get; set; }

    public string? Apellidos { get; set; }

    public string Correo { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public byte EstatusRegistro { get; set; }

    public int IdVerificacion { get; set; }

    public virtual DobleVerificacion IdVerificacionNavigation { get; set; } = null!;
}
