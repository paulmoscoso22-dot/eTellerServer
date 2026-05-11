namespace eTeller.Auth.ViewModels;

/// <summary>ViewModel risposta endpoint POST /api/auth/change-password.</summary>
public class ChangePasswordVm
{
    /// <summary>OK | INVALID_CURRENT_PASSWORD | POLICY_VIOLATION | HISTORY_VIOLATION | ERROR</summary>
    public string ResultCode { get; set; } = string.Empty;

    public string? Message { get; set; }
}
