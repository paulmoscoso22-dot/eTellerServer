using eTeller.Application.Features.User.DTOs;

namespace eTeller.Application.Features.User.Services
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<LoginResponseDto> ForceLoginAsync(LoginRequestDto request);
        Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordRequestDto request);
        Task<ValidateUserResponseDto> ValidateUserAsync(ValidateUserRequestDto request);
        Task<string?> WhoIsLoggedAsync(string ipAddress);
        Task<bool> LogoutAsync(string userId);
    }
}
