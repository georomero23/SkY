using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public class CCatalogoMaestro
{
    public int IdCatalogo { get; set; }

    public string? Nombre { get; set; }

    public string? InfoAdicional { get; set; }

    public bool? Disponible { get; set; }
    public CCatalogoOpcione[] Opciones { get; set; } = [];
}
