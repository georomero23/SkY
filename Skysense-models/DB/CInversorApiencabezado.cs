using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public class CInversorApiencabezado
{
    public int IdInversorApi { get; set; }

    public int? IdInstalacion { get; set; }

    public int? IdInversor { get; set; }
    public string? NumeroSerie { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public DateTime FechaUltimaActualizacion { get; set; }

    public string IdApi { get; set; } = null!;

    public IEnumerable<CInversorApigeneracion> CInversorApigeneracions { get; set; } = new List<CInversorApigeneracion>();
}
