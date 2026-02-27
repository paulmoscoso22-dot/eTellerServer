using eTeller.Application.Features.User.DTOs;

namespace eTeller.Application.Features.User.Commands
{
    public class LoginCommand
    {
        public required string UserId { get; set; }
        public required string Password { get; set; }
        public required string IpAddress { get; set; }
        public bool IsCashDesk { get; set; }
        public string? CashDeskId { get; set; }
        public string? BranchId { get; set; }
        public string? MacAddress { get; set; }
        public bool ForceLogin { get; set; }
        public bool IsNewSession { get; set; }
    }

    public class ChangePasswordCommand
    {
        public required string UserId { get; set; }
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }
    }

    public class ValidateUserQuery
    {
        public required string UserId { get; set; }
        public required string Password { get; set; }
    }
}
