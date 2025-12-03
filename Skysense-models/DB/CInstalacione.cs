using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Skysense_models.DB;

public class CInstalacione
{
    public int IdInstalacion { get; set; }

    public int IdCliente { get; set; }
    public int TipoProyecto { get; set; }

    public int? IdPlataforma { get; set; }

    public short? Zona { get; set; }

    public int? IdCatalogoEstatus { get; set; }

    public int? IdCatalogoTarifa { get; set; }

    public string? Nombre { get; set; }

    public string? CodigoProyecto { get; set; }

    public string? Longitud { get; set; }

    public string? Latitud { get; set; }

    public string? Estado { get; set; }

    public string? Ubicacion { get; set; }

    public DateOnly? GarantiaInicio { get; set; }

    public DateOnly? GarantiaFin { get; set; }

    public int? IdCatalogoGarantiaEstatus { get; set; }

    public DateOnly? InicioOperaciones { get; set; }

    public decimal? PorcentajeDesgastePaneles { get; set; }

    public string? IdInstalacionApi { get; set; }

    public string? Rpu { get; set; }

    public short? IdGrupo { get; set; }

    public int? AnioInstalacion { get; set; }

    public string? ContactoNombre { get; set; }

    public string? ContactoTelefono { get; set; }

    public string? ContactoEmail { get; set; }

    public decimal? PotenciaInstalada { get; set; }

    public bool? EsFinanciado { get; set; }

    public string GrupoNombre { get; set; } = string.Empty;

    public string sEstatus { get; set; } = string.Empty ;
    public string sTarifa { get; set; } = string.Empty ;

    public CCliente IdClienteNavigation { get; set; } = null!;

    public List<CInversore> Inversores { get; set; } = new List<CInversore>();

    public List<CPanele> Paneles { get; set; } = new List<CPanele>();

    public CGrupo? IdGrupoNavigation { get; set; }
}
