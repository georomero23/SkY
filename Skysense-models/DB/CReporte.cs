using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public class CReporte
{
    public int IdReporte { get; set; }
    public int IdInstalacion { get; set; }
    public DateOnly MesReporte { get; set; }
    public int? IdDocumento { get; set; }
    public CDocumento? IdDocumentoNavigation { get; set; }


    public decimal? PanelesGeneracion { get; set; }
    public decimal? AhorroAcumulado { get; set; }
    public decimal? AhorroAmbiental { get; set; }
    public decimal? ConsumoCFE { get; set; }
}
