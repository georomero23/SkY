using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public class CCliente
{
    public int IdCliente { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public byte IndicadorEstado { get; set; }

    public bool? EsEspecial { get; set; }

    public string? Rfc { get; set; }

    public string? Contacto { get; set; }

    public int CuantasInstalaciones { get; set; }
}
