using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public class CUser
{
    public int IdUsuario { get; set; }

    public byte UsEstado { get; set; }

    public string? UsNombre { get; set; }

    public string? UsApellidos { get; set; }

    public string UsCorreo { get; set; } = null!;

    public string UsCntrsn { get; set; } = null!;

    public DateTime UsFechaAlta { get; set; }
    public bool UsCambiaCntrsn { get; set; }

    public byte? IdRol { get; set; }
}
