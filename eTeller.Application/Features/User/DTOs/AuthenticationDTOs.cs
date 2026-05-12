namespace eTeller.Application.Features.User.DTOs
{
    public class LoginRequestDto
    {
        public required string UserId { get; set; }
        public required string Password { get; set; }
        public string? IpAddress { get; set; }
        public bool IsCashDesk { get; set; }
        public string? CashDeskId { get; set; }
        public string? BranchId { get; set; }
        public string? MacAddress { get; set; }
        public bool IsNewSession { get; set; }
    }

    public class LoginResponseDto
    {
        public bool IsSuccessful { get; set; }
        public required string ResultCode { get; set; }
        public string? Message { get; set; }
        public UserSessionDto? UserSession { get; set; }
        public bool RequiresPasswordChange { get; set; }
        public bool UserAlreadyLogged { get; set; }
        public DateTime? ExistingLoginDate { get; set; }
    }

    public class UserSessionDto
    {
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public required string BranchId { get; set; }
        public required string Language { get; set; }
        public bool CanUseTeller { get; set; }
        public string? CashDeskId { get; set; }
        public string? CashDeskStatus { get; set; }
        public string? CashDeskLanguage { get; set; }
        public string? CashDeskDescription { get; set; }
        public string? IpAddress { get; set; }
        public DateTime SessionStartTime { get; set; }
        public DateTime SessionExpirationTime { get; set; }
    }

    public class ChangePasswordRequestDto
    {
        public required string UserId { get; set; }
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }
    }

    public class ChangePasswordResponseDto
    {
        public bool IsSuccessful { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }

    public class ValidateUserRequestDto
    {
        public required string UserId { get; set; }
        public required string Password { get; set; }
    }

    public class ValidateUserResponseDto
    {
        public bool IsValid { get; set; }
        public bool IsBlocked { get; set; }
        public bool RequiresPasswordChange { get; set; }
        public string? Message { get; set; }
    }
}
