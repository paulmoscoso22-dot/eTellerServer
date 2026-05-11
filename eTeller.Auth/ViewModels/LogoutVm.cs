namespace eTeller.Auth.ViewModels;

/// <summary>ViewModel risposta endpoint POST /api/auth/logout.</summary>
public class LogoutVm
{
    /// <summary>OK | ERROR</summary>
    public string ResultCode { get; set; } = string.Empty;

    public string? Message { get; set; }
}
