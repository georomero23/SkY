using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public class CDocumento
{
    public int IdDocumento { get; set; }
    public int IdInstalacion { get; set; }

    public byte TipoDocumento { get; set; }

    public string NombreDocumento { get; set; } = null!;
    public string TipoArchivo { get; set; } = null!;
    public int Peso { get; set; }

    public string RutaRelativa { get; set; } = null!;

    public DateTime FechaModificacion { get; set; }
}
