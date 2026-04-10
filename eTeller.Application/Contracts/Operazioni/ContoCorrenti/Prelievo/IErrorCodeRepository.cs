using eTeller.Domain.Models;

namespace eTeller.Application.Contracts.Operazioni.ContoCorrenti.Prelievo;

/// <summary>
/// Contratto per l'accesso ai codici errore del sistema eTeller.
/// I dati sono letti dalla tabella ST_ERRORCODE tramite la stored procedure
/// [dbo].[st_ERRORCODE_SelectByID].
/// </summary>
public interface IErrorCodeRepository
{
    /// <summary>
    /// Restituisce la descrizione dell'errore nella lingua specificata.
    /// Se la traduzione non è disponibile, fa fallback all'italiano.
    /// Se l'italiano non è disponibile, restituisce il codice errore grezzo.
    /// </summary>
    /// <param name="errorCode">Codice errore (es. "1305", "9024")</param>
    /// <param name="lingua">
    /// Codice lingua ISO 639-1 (es. "IT", "EN", "FR", "DE").
    /// Default "IT" se non specificato.
    /// </param>
    /// <returns>Descrizione localizzata dell'errore</returns>
    string GetDescription(string errorCode, string lingua = "IT");

    /// <summary>
    /// Restituisce la descrizione dell'errore in modo asincrono nella lingua specificata.
    /// </summary>
    /// <param name="errorCode">Codice errore (es. "1305", "9024")</param>
    /// <param name="lingua">
    /// Codice lingua ISO 639-1 (es. "IT", "EN", "FR", "DE").
    /// Default "IT" se non specificato.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<string> GetDescriptionAsync(
        string errorCode,
        string lingua = "IT",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restituisce l'entità ErrorCode completa con tutti i campi (flags, soluzione, ecc.).
    /// Utile quando serve più della sola descrizione (es. per decidere se bloccare o avvisare).
    /// Restituisce null se il codice non esiste.
    /// </summary>
    /// <param name="errorCode">Codice errore (es. "1305")</param>
    /// <param name="cancellationToken"></param>
    Task<ErrorCode?> GetByIdAsync(
        string errorCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se un codice errore esiste nella tabella ST_ERRORCODE.
    /// </summary>
    /// <param name="errorCode">Codice errore da verificare</param>
    /// <param name="cancellationToken"></param>
    Task<bool> ExistsAsync(
        string errorCode,
        CancellationToken cancellationToken = default);
}
