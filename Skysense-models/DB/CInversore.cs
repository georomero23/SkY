using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Skysense_models.DB;

public class CInversore
{
    public int IdInversor { get; set; }

    public int IdInstalacion { get; set; }

    public byte IdEstadoInversor { get; set; }

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
