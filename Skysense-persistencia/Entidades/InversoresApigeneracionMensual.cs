using System;
using System.Collections.Generic;

namespace Skysense_persistencia.Entidades;

public partial class InversoresApigeneracionMensual
{
    public int? IdInstalacion { get; set; }

    public byte Mes { get; set; }

    public short Anno { get; set; }

    public decimal? GeneracionGarantizada { get; set; }

    public bool DaemonYaEjecutado { get; set; }

    public virtual Instalacione? IdInstalacionNavigation { get; set; }
}
