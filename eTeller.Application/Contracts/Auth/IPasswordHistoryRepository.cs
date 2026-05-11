namespace eTeller.Application.Contracts.Auth;

/// <summary>
/// Porta per la gestione dello storico password (tabella sys_USER_PASSWD).
/// Implementazione Dapper in eTeller.Infrastructure.Auth.PasswordHistoryRepository.
/// </summary>
public interface IPasswordHistoryRepository
{
    /// <summary>
    /// Verifica se la password fornita in chiaro è già presente nelle ultime <paramref name="maxHistory"/> password dell'utente.
    /// Controlla sia hash BCrypt che hash MD5 legacy.
    /// </summary>
    Task<bool> IsPasswordInHistoryAsync(string userId, string plainPassword, int maxHistory);

    /// <summary>
    /// Inserisce la nuova password (già hashed) nello storico sys_USER_PASSWD.
    /// Corrisponde a SP: sys_USER_PASSWD_Insert_Password
    /// </summary>
    Task InsertPasswordAsync(string userId, string passwordHash, DateTime modDate);

    /// <summary>
    /// Elimina le entry di storico eccedenti il limite configurato.
    /// Corrisponde a SP: sys_USER_PASSWD_Clear_Password_History
    /// </summary>
    Task ClearOldPasswordsAsync(string userId, int maxHistory);
}
