using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public class CPanele
{
    public int IdPanel { get; set; }

    public int IdInstalacion { get; set; }

    public byte IdEstadoPanel { get; set; }

    public string NumeroSerie { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public decimal Potencia { get; set; }

    public decimal DegradacionAnual { get; set; }

    public string ProveedorSuministrador { get; set; } = null!;

    public string ProveedorSuministradorRfc { get; set; } = null!;

    public string ProveedorIntermediario { get; set; } = null!;

    public string ProveedorIntermediarioRfc { get; set; } = null!;

    public bool EsInicial { get; set; }

    public bool Danado { get; set; }

    public string? NumeroSerieReemplazo { get; set; }
}
