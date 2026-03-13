using FluentResults;
using MediatR;

namespace eTeller.Application.Features.ContiCorrenti.Commands.Carica;

/// <summary>Comando per caricare/modificare una transazione su conto corrente</summary>
public sealed record CaricaContiCorrentiCommand : IRequest<Result<CaricaContiCorrentiResult>>
{
    // ── Dati conto ────────────────────────────────────────────────────────────
    
    /// <summary>Numero identificativo del conto corrente (es. "CH123456")</summary>
    public string NumeroConto { get; init; } = string.Empty;

    /// <summary>Codice tipo conto (es. "CA" = conto corrente, "SB" = risparmio)</summary>
    public string TipoContoCode { get; init; } = string.Empty;

    /// <summary>Divisa del conto (3 lettere ISO 4217, es. "EUR")</summary>
    public string DivisaConto { get; init; } = string.Empty;

    // ── Dati operazione ───────────────────────────────────────────────────────

    /// <summary>Tipo operazione: "DEP" = versamento, "WITH" = prelievo</summary>
    public string TipoOperazione { get; init; } = string.Empty;

    /// <summary>
    /// Importo da addebitare/accreditare sul conto nella divisa del conto.
    /// Null se l'utente ha inserito solo l'importo banconote (IMPCTP).
    /// </summary>
    public decimal? ImportoConto { get; init; }

    /// <summary>
    /// Importo in banconote nella divisa banconote.
    /// Null se l'utente ha inserito solo l'importo conto (IMPOPE).
    /// </summary>
    public decimal? ImportoBanconote { get; init; }

    // ── Dati banconote ────────────────────────────────────────────────────────

    /// <summary>Divisa delle banconote (3 lettere ISO 4217, es. "USD")</summary>
    public string DivisaBanconote { get; init; } = string.Empty;

    /// <summary>Tasso di cambio applicato dall'operatore tra divisa conto e banconote</summary>
    public decimal? TassoCambio { get; init; }

    /// <summary>Tasso di cambio per il controvalore CHF (cambio CTV)</summary>
    public decimal? TassoCambioControvalore { get; init; }

    // ── Aggio ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Valore aggio: percentuale se TipoAggioIsPercentuale = true, altrimenti importo fisso.
    /// Null se non applicato.
    /// </summary>
    public decimal? Aggio { get; init; }

    /// <summary>True = aggio espresso in %, False = aggio espresso come importo fisso</summary>
    public bool TipoAggioIsPercentuale { get; init; } = true;

    /// <summary>Segno aggio: 1 = aggio (maggiorazione), -1 = disaggio (riduzione)</summary>
    public int SegnoAggio { get; init; } = 1;

    // ── Date ─────────────────────────────────────────────────────────────────

    /// <summary>Data valuta dell'operazione</summary>
    public DateOnly DataValuta { get; init; }

    /// <summary>Data operazione (sola lettura, valorizzata dal sistema)</summary>
    public DateTime DataOperazione { get; init; }

    // ── Opzioni stampa ────────────────────────────────────────────────────────

    public bool StampaSaldo { get; init; }
    public bool StampaAvviso { get; init; }

    // ── Forza cambio ─────────────────────────────────────────────────────────

    /// <summary>True se l'operatore ha forzato un cambio fuori tolleranza</summary>
    public bool ForzaCambio { get; init; }

    /// <summary>Commento interno obbligatorio se ForzaCambio = true</summary>
    public string? CommentoInterno { get; init; }

    /// <summary>Nome e cognome obbligatorio se ForzaCambio = true</summary>
    public string? NomeCognome { get; init; }

    // ── Arrotondamento ────────────────────────────────────────────────────────

    /// <summary>Se true, l'importo banconote viene arrotondato al taglio minimo della valuta</summary>
    public bool ArrotondaTaglioMinimo { get; init; } = true;

    // ── Antiriciclaggio ───────────────────────────────────────────────────────

    /// <summary>Dati comparente antiriciclaggio, null se non richiesti</summary>
    public AntiRiciclaggioDto? DatiAntiRiciclaggio { get; init; }

    // ── Contesto utente ───────────────────────────────────────────────────────

    /// <summary>ID utente che esegue l'operazione</summary>
    public string UserId { get; init; } = string.Empty;

    /// <summary>Codice cassa dell'operatore</summary>
    public string CodiceCassa { get; init; } = string.Empty;

    /// <summary>Codice sede/filiale</summary>
    public string CodiceFiliale { get; init; } = string.Empty;

    // ── ID transazione (solo per modifica) ───────────────────────────────────

    /// <summary>
    /// ID della transazione esistente in caso di modifica.
    /// 0 o null indica una nuova transazione.
    /// </summary>
    public int? TransactionId { get; init; }
}

/// <summary>Dati antiriciclaggio/vigilanza allegati al comando.</summary>
public sealed record AntiRiciclaggioDto
{
    public string NomeComparente { get; init; } = string.Empty;
    public string DocumentoTipo { get; init; } = string.Empty;
    public string DocumentoNumero { get; init; } = string.Empty;
    public bool Forzato { get; init; }
}
