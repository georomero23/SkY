using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public class CReporteAutomaticoConfig
{
    public int IdInstalacion { get; set; }

    public string NombreEnRecibo { get; set; }

    public decimal? PorcentajeDap { get; set; }

    public decimal? UmbralFp { get; set; }

    public decimal? FpDefault { get; set; }
}
