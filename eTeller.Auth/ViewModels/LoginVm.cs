namespace eTeller.Auth.ViewModels;

/// <summary>
/// ViewModel risposta endpoint POST /api/auth/login e /api/auth/force-login.
/// </summary>
public class LoginVm
{
    /// <summary>Codice risultato interno (per logging). Non inviare direttamente al client il codice differenziato per USER_NOT_FOUND vs INVALID_CREDENTIALS.</summary>
    public string ResultCode { get; set; } = string.Empty;

    /// <summary>Messaggio leggibile da mostrare al client. Mai distinguere USER_NOT_FOUND da INVALID_CREDENTIALS.</summary>
    public string? Message { get; set; }

    /// <summary>JWT access token (presente solo se ResultCode = "OK").</summary>
    public string? AccessToken { get; set; }

    /// <summary>Data/ora scadenza token (UTC).</summary>
    public DateTime? TokenExpiresAt { get; set; }

    /// <summary>True se la password è scaduta o deve essere cambiata obbligatoriamente.</summary>
    public bool RequiresPasswordChange { get; set; }

    /// <summary>True se l'utente è già loggato da altro IP. Il client deve proporre ForceLogin.</summary>
    public bool UserAlreadyLogged { get; set; }

    /// <summary>True se la cassa è già occupata da altro utente.</summary>
    public bool CashDeskBusy { get; set; }

    public LoginSessionVm? UserSession { get; set; }
}

/// <summary>Dati di sessione inclusi nel LoginVm quando il login è OK.</summary>
public class LoginSessionVm
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string BranchId { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public bool CanUseTeller { get; set; }
    public string? CashDeskId { get; set; }
    public string? CashDeskDescription { get; set; }
    public string? CashDeskBranchId { get; set; }
}
