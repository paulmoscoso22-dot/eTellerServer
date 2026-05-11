namespace eTeller.Application.Contracts.Auth;

/// <summary>
/// Claims del token JWT emesso al login.
/// Mappatura dal FormsAuthenticationTicket del sistema legacy eTeller2022.
/// </summary>
public record UserTokenClaims(
    string UserId,            // sub  — chiave primaria utente (USR_ID)
    string UserName,          // name — nome visualizzato (USR_EXTREF)
    string BranchId,          // branch_id — filiale utente (USR_BRA_ID)
    string Language,          // language — lingua interfaccia (USR_LINGUA)
    bool CanUseTeller,        // can_use_teller — flag cassa
    string? CashDeskId,       // cash_desk_id — id cassa (nullable se non è postazione cassa)
    string? CashDeskBranchId, // cash_desk_branch_id — filiale cassa (nullable)
    string IpAddress,         // ip_address — IP del client al momento del login
    string SessionId          // session_id — identificativo univoco sessione
);
