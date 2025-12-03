using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public partial class CDatosInversoresAjustado
{
    public int IdAjuste { get; set; }

    public int? IdInstalacion { get; set; }

    public string? IdentificadorInversor { get; set; }

    public string? IdUsuarioModificacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public DateOnly? FechaRegistro { get; set; }

    public decimal? ValorOriginal { get; set; }

    public decimal? ValorNuevo { get; set; }
}
