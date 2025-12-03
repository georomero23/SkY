using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public class CRecibo
{
    public int IdRecibo { get; set; }
    public int IdInstalacion { get; set; }
    public DateOnly MesRecibo { get; set; }
    public int? IdDocumento { get; set; }
    public decimal? KWhBase { get; set; }

    public decimal? KWhIntermedia { get; set; }

    public decimal? KWhPunta { get; set; }

    public decimal? KWbase { get; set; }

    public decimal? KWintermedia { get; set; }

    public decimal? KWpunta { get; set; }

    public decimal? ReactivosKvArh { get; set; }
    public decimal? Pago { get; set; }
    public decimal? AlumbradoPublicoC { get; set; }
    public decimal? AlumbradoPublicoS { get; set; }
    public CDocumento? IdDocumentoNavigation { get; set; }

}
