namespace eTeller.Domain.Services;

/// <summary>
/// Domain Service per le operazioni di verifica IS107 (limiti di operatività).
/// Gestisce il controllo dei limiti configurati e lo stato delle segnalazioni.
/// </summary>
public interface IIS107DomainService
{
    /// <summary>
    /// Verifica se la transazione proposte rispetta i limiti IS107 configurati.
    /// </summary>
    /// <param name="request">Dati della verifica</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Risultato della verifica con esito, messaggi e dettagli limite</returns>
    Task<IS107VerificaResult> VerificaLimitiAsync(
        IS107VerificaRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>Request per la verifica IS107.</summary>
public sealed record IS107VerificaRequest
{
    /// <summary>ID transazione (0 se nuova)</summary>
    public int TransactionId { get; init; }

    /// <summary>Numero relazione del cliente</summary>
    public string NumeroRelazione { get; init; } = string.Empty;

    /// <summary>Flag IS107 del cliente (Y/N)</summary>
    public string FlagIS107 { get; init; } = string.Empty;

    /// <summary>Tipo operazione: "DEP" o "WITH"</summary>
    public string TipoOperazione { get; init; } = string.Empty;

    /// <summary>Importo controvalore CHF della transazione</summary>
    public decimal ImportoControvaloreChf { get; init; }

    /// <summary>Patrimonio del cliente</summary>
    public decimal Patrimonio { get; init; }

    /// <summary>Commento interno inserito dall'operatore (per override)</summary>
    public string? CommentoInterno { get; init; }

    /// <summary>Importo della transazione precedente (per modifica)</summary>
    public decimal? ImportoPrecedente { get; init; }
}

/// <summary>Risultato della verifica IS107.</summary>
public sealed record IS107VerificaResult
{
    /// <summary>Esito della verifica</summary>
    public IS107EsitoVerifica Esito { get; init; }

    /// <summary>Messaggio descrittivo dell'esito</summary>
    public string? Messaggio { get; init; }

    /// <summary>Somma totale delle transazioni (per warning)</summary>
    public decimal? SommaTotale { get; init; }

    /// <summary>Limite configurato (per warning/block)</summary>
    public decimal? LimiteConfigurato { get; init; }
}

/// <summary>Enumerazione degli esiti possibili della verifica IS107.</summary>
public enum IS107EsitoVerifica
{
    /// <summary>Operazione consentita, nessun limite superato</summary>
    Ok = 0,

    /// <summary>Limite superato, operazione bloccata</summary>
    Block = 1,

    /// <summary>Limite superato, richiesta un commento per procedere</summary>
    Warning = 2,

    /// <summary>Importo della transazione è stato modificato dal sistema</summary>
    ImportoModificato = 3
}
