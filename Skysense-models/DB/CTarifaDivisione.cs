using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public class CTarifasDivisione
{
    public int IdTarifa { get; set; }

    public int IdDivision { get; set; }

    public short Anno { get; set; }

    public byte Mes { get; set; }

    public decimal? ValorTransmision { get; set; }

    public decimal? ValorDistribucion { get; set; }

    public decimal? ValorOpCenace { get; set; }

    public decimal? ValorOpSsb { get; set; }

    public decimal? ValorServiciosNoMem { get; set; }

    public decimal? ValorEnergiaBase { get; set; }

    public decimal? ValorEnergiaIntermedia { get; set; }

    public decimal? ValorEnergiaPunta { get; set; }

    public decimal? ValorEnergiaSemipunta { get; set; }

    public decimal? ValorCapacidad { get; set; }
}
