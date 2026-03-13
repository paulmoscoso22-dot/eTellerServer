namespace eTeller.Domain.Services;

/// <summary>
/// Domain Service per le operazioni di calcolo Forex e validazioni sui cambi.
/// Astrae la logica di business pura legata ai tassi di cambio, spread,
/// tagli minimi e tolleranze.
/// </summary>
public interface IForexDomainService
{
    /// <summary>
    /// Verifica che l'importo rispetti il taglio minimo della valuta per i Biglietti Banca.
    /// </summary>
    /// <param name="codiceDivisa">Codice ISO 4217 della divisa (es. "USD")</param>
    /// <param name="importo">Importo da verificare</param>
    /// <param name="cancellationToken"></param>
    /// <returns>True se l'importo rispetta il taglio minimo</returns>
    Task<bool> RispettaTaglioMinimoAsync(
        string codiceDivisa,
        decimal importo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restituisce il taglio minimo configurato per la divisa specificata.
    /// </summary>
    Task<decimal> GetTaglioMinimoAsync(
        string codiceDivisa,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcola il cambio cross tra due divise applicando eventualmente lo spread
    /// in base al tipo operazione e alla categoria del conto.
    /// </summary>
    /// <param name="divisaConto">Divisa del conto (es. "EUR")</param>
    /// <param name="divisaBanconote">Divisa delle banconote (es. "USD")</param>
    /// <param name="tipoOperazione">"DEP" o "WITH"</param>
    /// <param name="applicaSpread">True se lo spread deve essere applicato</param>
    /// <param name="isDipendente">True se il conto è di un dipendente (cat. 4)</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dati del cambio calcolato</returns>
    Task<CambioCalcolatoDto> CalcolaCambioAsync(
        string divisaConto,
        string divisaBanconote,
        string tipoOperazione,
        bool applicaSpread,
        bool isDipendente,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica che il tasso inserito dall'operatore rientri nella tolleranza
    /// rispetto al tasso di sistema. Tiene conto di ForzaCambio.
    /// </summary>
    /// <param name="codiceDivisa">Divisa da controllare</param>
    /// <param name="tassoOperatore">Tasso inserito dall'operatore</param>
    /// <param name="tassoDiSistema">Tasso di sistema calcolato</param>
    /// <param name="tipoOperazione">"DEP" o "WITH"</param>
    /// <param name="forzaCambio">True se il cambio è forzato dall'operatore</param>
    /// <returns>True se il tasso è fuori tolleranza (richiede blocco o commento)</returns>
    Task<bool> IsFuoriTolleranzaAsync(
        string codiceDivisa,
        decimal tassoOperatore,
        decimal tassoDiSistema,
        string tipoOperazione,
        bool forzaCambio,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Arrotonda l'importo al taglio minimo della divisa per i Biglietti Banca.
    /// </summary>
    Task<decimal> ArrotondaAlTaglioMinimoAsync(
        string codiceDivisa,
        decimal importo,
        bool applicaArrotondamento,
        CancellationToken cancellationToken = default);
}

/// <summary>DTO con i risultati del calcolo cambio cross.</summary>
public sealed record CambioCalcolatoDto
{
    /// <summary>Prezzo del cambio tra le due divise</summary>
    public decimal Prezzo { get; init; }

    /// <summary>Prezzo della divisa 1 in CHF (per calcolo CTV)</summary>
    public decimal PrezzoDivisa1 { get; init; }

    /// <summary>Prezzo della divisa 2 in CHF (per calcolo CTV)</summary>
    public decimal PrezzoDivisa2 { get; init; }

    /// <summary>Scala della divisa 1 (es. 100 per JPY)</summary>
    public decimal ScalaDivisa1 { get; init; }

    /// <summary>Scala della divisa 2</summary>
    public decimal ScalaDivisa2 { get; init; }

    /// <summary>
    /// Direzione del cambio:
    /// true  = prezzo = divisa1 / divisa2 (es. EUR/USD: moltiplicare)
    /// false = prezzo = divisa2 / divisa1 (es. USD/CHF: dividere)
    /// </summary>
    public bool Direzione { get; init; }

    /// <summary>Quantità decimali divisa 1</summary>
    public decimal DecimaliDivisa1 { get; init; }

    /// <summary>Quantità decimali divisa 2</summary>
    public decimal DecimaliDivisa2 { get; init; }

    /// <summary>True se il cambio è un cross (nessuna divisa è CHF)</summary>
    public bool IsCross => PrezzoDivisa1 != 0 && PrezzoDivisa2 != 0;
}
