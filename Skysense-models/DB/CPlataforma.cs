using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public class CPlataforma
{
    public int IdPlataforma { get; set; }

    public string Nombre { get; set; } = null!;

    public bool? Disponible { get; set; }
}
