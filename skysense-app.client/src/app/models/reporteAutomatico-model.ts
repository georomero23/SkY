export class ReporteAutomaticoDatosModel{
        idInstalacion:number = 0;
        iAnno :number = 0;
        iMes :number = 0;
        cliente :string = "";
        direccion :string = "";
        rpu :string = "";
        periodo :string = "";
        nombreEnRecibo :string = "";
        capacidadInstalada :number = 0;
        pagoSinPaneles :number = 0;
        pagoConPaneles :number = 0;
        ahorro :number = 0;
        porcentajeAhorro :number = 0;
        consumoTotal :number = 0;
        consumoCFE :number = 0;
        consumoPaneles :number = 0;
        consumoTotalPorcentaje :number = 0;
        consumoCFEPorcentaje :number = 0;
        consumoPanelesPorcentaje :number = 0;
        generacionPeriodo :number = 0;
        fijoCPaneles :number = 0;
        fijoSPaneles :number = 0;
        fijoAhorro :number = 0;
        baseCPaneles :number = 0;
        baseSPaneles :number = 0;
        baseAhorro :number = 0;
        intermediaCPaneles :number = 0;
        intermediaSPaneles :number = 0;
        intermediaAhorro :number = 0;
        puntaCPaneles :number = 0;
        puntaSPaneles :number = 0;
        puntaAhorro :number = 0;
        transmisionCPaneles :number = 0;
        transmisionSPaneles :number = 0;
        transmisionAhorro :number = 0;
        cenacecPaneles :number = 0;
        cenacesPaneles :number = 0;
        cenaceAhorro :number = 0;
        scnmemcPaneles :number = 0;
        scnmemsPaneles :number = 0;
        scnmemAhorro :number = 0;
        distribucionCPaneles :number = 0;
        distribucionSPaneles :number = 0;
        distribucionAhorro :number = 0;
        capacidadCPaneles :number = 0;
        capacidadSPaneles :number = 0;
        capacidadAhorro :number = 0;
        energiaCPaneles :number = 0;
        energiaSPaneles :number = 0;
        energiaAhorro :number = 0;
        factorPotenciaCPaneles :number = 0;
        factorPotenciaSPaneles :number = 0;
        factorPotenciaAhorro :number = 0;
        subtotalCPaneles :number = 0;
        subtotalSPaneles :number = 0;
        subtotalAhorro :number = 0;
        dapcPaneles :number = 0;
        dapsPaneles :number = 0;
        dapahorro :number = 0;
        ivacPaneles :number = 0;
        ivasPaneles :number = 0;
        ivAhorro :number = 0;
        totalCPaneles :number = 0;
        totalSPaneles :number = 0;
        totalAhorro :number = 0;
        ahorroAcumuladoSIva :number = 0;
        aporteArboles :string = "0 árboles.";
        aporteCO2 :number = 0;
        ahorroAcumuladoDesde :string = "";
        porcentajeCumplimiento :number = 0;
        generacionDiaria :number[] = [];
        historicoGenPVEsteAnnio :number[] = [];
        historicoGenPVAnnioAnterior :number[] = [];
        historicoConsumo :number[] = [];
        historicoFacturas: number[][] = [];

  bajaTension2C: number = 0;
  bajaTension2S: number = 0;
  bajaTension2Ahorro: number = 0;

  constructor(init?: Partial<ReporteAutomaticoConfig>)
 {
    Object.assign(this, init);
  }

}

export class ReporteAutomaticoConfig{
    idInstalacion: number = 0;

    nombreEnRecibo: string = "";

    porcentajeDap: number = 0.00;
    porcentajeDapNumeros: number = 0.00;

    umbralFp:number  = 0.00;

    fpDefault:number = 0.90;
  fpDefaultNumeros: number = 0.90;

  bajaTension2: boolean = false;

  constructor(init?: Partial<ReporteAutomaticoConfig>) {
    if (init) {
      Object.assign(this, init);
      this.bajaTension2 = !!init.bajaTension2;
    }
  }

}
