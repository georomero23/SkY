using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public class CCatalogoOpcione
{
    public int IdCatalogo { get; set; }

    public int IdOpcion { get; set; }

    public string? NombreOpcion { get; set; }

    public string? InfoAdicional { get; set; }

    public bool Disponible { get; set; }
}
