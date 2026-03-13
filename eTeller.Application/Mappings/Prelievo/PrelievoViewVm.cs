using eTeller.Application.Share;
using eTeller.Domain.Models.View;

namespace eTeller.Application.Mappings.Prelievo;

/// <summary>
/// ViewModel for withdrawal (prelievo) operations
/// Maps from PrelievoView domain model
/// </summary>
public class PrelievoViewVm
{
    /// <summary>Numero del conto corrente (ex: tbConto)</summary>
    public string NumeroConto { get; set; } = string.Empty;

    /// <summary>Tipo conto — es. "CA" = conto corrente, "SB" = risparmio (ex: accountTypeCode)</summary>
    public string TipoConto { get; set; } = string.Empty;

    /// <summary>Divisa del conto — es. "CHF", "EUR" (ex: DdlTRX_CTOOPECUR)</summary>
    public string DivisaConto { get; set; } = string.Empty;

    // --- Sezione Banconote (Biglietti Banca) ---

    /// <summary>Divisa delle banconote fisiche — es. "USD" (ex: DdlTRX_CTOCTCUR)</summary>
    public string DivisaContante { get; set; } = string.Empty;

    /// <summary>
    /// Importo addebitato/accreditato sul conto nella divisa del conto.
    /// Può essere null se il cassiere ha inserito solo ImportoContante (ex: TbTRX_IMPOPE)
    /// </summary>
    public decimal? ImportoConto { get; set; }

    /// <summary>
    /// Importo in banconote fisiche nella divisa contante.
    /// Può essere null se il cassiere ha inserito solo ImportoConto (ex: TbTRX_IMPCTP)
    /// </summary>
    public decimal? ImportoContante { get; set; }

    // --- Sezione Cambio ---

    /// <summary>
    /// Tasso di cambio inserito manualmente dal cassiere (ex: TbTRX_EXCRAT).
    /// Null = verrà calcolato automaticamente dal sistema.
    /// </summary>
    public decimal? TassoCambio { get; set; }

    /// <summary>
    /// Tasso di cambio per il controvalore in CHF (ex: txtCambioCTV).
    /// Null = verrà calcolato automaticamente.
    /// </summary>
    public decimal? TassoControvalore { get; set; }

    /// <summary>Il cassiere ha forzato il cambio fuori tolleranza? (ex: cbForceChange)</summary>
    public bool ForzaCambio { get; set; } = false;

    // --- Sezione Aggio ---

    /// <summary>
    /// Valore dell'aggio. Null = nessun aggio applicato (ex: tbAggio).
    /// </summary>
    public decimal? Aggio { get; set; }

    /// <summary>
    /// Tipo di aggio: "percentuale" oppure "importo" (ex: rblAggio).
    /// </summary>
    public TipoAggioEnum TipoAggio { get; set; } = TipoAggioEnum.Percentuale;

    /// <summary>
    /// Segno dell'aggio: "aggio" (positivo) oppure "disaggio" (negativo) (ex: rblAggioTipo).
    /// </summary>
    public SegnoAggioEnum SegnoAggio { get; set; } = SegnoAggioEnum.Aggio;

    // --- Sezione Date ---

    /// <summary>Data valuta dell'operazione (ex: TbTRX_DATVAL)</summary>
    public DateOnly DataValuta { get; set; }

    // --- Sezione Stampa ---

    /// <summary>Stampare il saldo sul documento? (ex: ckPrintSaldo)</summary>
    public bool StampaSaldo { get; set; } = false;

    /// <summary>Stampare l'avviso? (ex: ckPrintAdvise)</summary>
    public bool StampaAvviso { get; set; } = false;

    // --- Sezione Note ---

    /// <summary>Commento interno obbligatorio in caso di cambio forzato (ex: pnlOsservazioni.InternalComment)</summary>
    public string? CommentoInterno { get; set; }

    /// <summary>Nome e cognome del cliente presentato allo sportello (ex: pnlOsservazioni.NomeCognome)</summary>
    public string? NomeCognome { get; set; }

    /// <summary>Testo libero aggiuntivo (ex: pnlOsservazioni.TestoLibero)</summary>
    public string? TestoLibero { get; set; }

    // --- Tipo operazione ---

    /// <summary>Tipo operazione: DEP = versamento, WITH = prelevamento (ex: operationType)</summary>
    public TipoOperazioneEnum TipoOperazione { get; set; }

    /// <summary>
    /// ID transazione esistente. Zero = nuova transazione, >0 = modifica (ex: operationNmb)
    /// </summary>
    public int TransazioneId { get; set; } = 0;

    /// <summary>Arrotondare al taglio minimo della banconota? (ex: cbArrotondaImporti)</summary>
    public bool ArrotondaTaglioMinimo { get; set; } = true;
}
