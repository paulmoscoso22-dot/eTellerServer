namespace eTeller.Auth.Contracts;

/// <summary>
/// Porta per la gestione delle sessioni attive (tabella sys_USERSuseClient).
/// Richiede Dapper per join multi-tabella.
/// Implementazione in eTeller.Infrastructure.Auth.SessionRepository.
/// </summary>
public interface ISessionRepository
{
    /// <summary>
    /// Verifica se esiste una sessione attiva per l'utente su una specifica cassa o su qualsiasi IP.
    /// Corrisponde a sys_USERSuseClient_SelectByUSR_CLI_LOG.
    /// </summary>
    Task<SessionInfo?> GetActiveSessionAsync(string userId, string? cashDeskId);

    /// <summary>
    /// Registra una nuova sessione utente.
    /// Corrisponde a EnterUser2 legacy / sys_USERSuseClient_Insert.
    /// Restituisce il GUID della sessione creata.
    /// </summary>
    Task<string> InsertSessionAsync(string userId, string cashDeskId, string ipAddress, DateTime loginTime);

    /// <summary>
    /// Chiude una sessione esistente (logout).
    /// Corrisponde a ExitUser2 legacy / sys_USERSuseClient_UpdateExit.
    /// </summary>
    Task CloseSessionAsync(string sessionId);

    /// <summary>
    /// Chiude tutte le sessioni attive di un utente (usato da ForceLogin).
    /// </summary>
    Task CloseAllSessionsForUserAsync(string userId);
}

/// <summary>Dati di una sessione attiva.</summary>
public record SessionInfo(
    string SessionId,
    string UserId,
    string? CashDeskId,
    string IpAddress,
    DateTime LoginTime
);
