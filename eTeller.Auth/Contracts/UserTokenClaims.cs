namespace eTeller.Auth.Contracts;

/// <summary>
/// Claims del token JWT emesso al login.
/// Corrisponde ai campi del FormsAuthenticationTicket del sistema legacy eTeller2022.
/// </summary>
public record UserTokenClaims(
    string UserId,           // sub — chiave primaria utente
    string UserName,         // name — nome completo
    string BranchId,         // branch_id — USR_BRA_ID
    string Language,         // language — USR_LINGUA
    bool CanUseTeller,       // can_use_teller — flag cassa
    string? CashDeskId,      // cash_desk_id — CLI_ID (nullable se non è cassa)
    string? CashDeskBranchId,// cash_desk_branch_id — BRA_ID cassa
    string IpAddress,        // ip_address — IP del client al login
    string SessionId         // session_id — GUID sessione (corrisponde a sys_USERSuseClient PK)
);
