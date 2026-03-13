namespace eTeller.Application.Contracts;

/// <summary>
/// Repository per la gestione dei codici di errore/risposta dell'host.
/// Fornisce traduzioni dei codici in messaggi leggibili all'utente.
/// </summary>
public interface IErrorCodeRepository
{
    /// <summary>
    /// Recupera la descrizione di un codice di errore nella lingua specificata.
    /// </summary>
    /// <param name="errorCode">Codice di errore (es. "T9", "T11", "0001")</param>
    /// <param name="languageCode">Codice ISO della lingua (es. "IT", "EN")</param>
    /// <param name="ct">Token di cancellazione</param>
    /// <returns>Descrizione del codice di errore, null se non trovato</returns>
    Task<string?> GetDescriptionAsync(string errorCode, string languageCode, CancellationToken ct = default);
}
