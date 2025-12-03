using System;
using System.Collections.Generic;

namespace Skysense_models.DB;

public class CInversorApigeneracion
{
    public int IdInversorApi { get; set; }

    public int IdConsecutivo { get; set; }

    public DateTime FechaValor { get; set; }
    public DateTime FechaUltimaActualizacion { get; set; }

    public decimal Valor { get; set; }

    public bool ValorManual { get; set; }
}
