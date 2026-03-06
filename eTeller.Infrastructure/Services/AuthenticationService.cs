using eTeller.Application.Contracts;
using eTeller.Application.Features.User.Commands;
using eTeller.Application.Features.User.DTOs;
using eTeller.Application.Features.User.Handlers;

namespace eTeller.Application.Features.User.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LoginCommandHandler _loginHandler;
        private readonly ChangePasswordCommandHandler _changePasswordHandler;

        public AuthenticationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _loginHandler = new LoginCommandHandler(unitOfWork);
            _changePasswordHandler = new ChangePasswordCommandHandler(unitOfWork);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var command = new LoginCommand
            {
                UserId = SanitizeInput(request.UserId),
                Password = request.Password,
                IpAddress = request.IpAddress,
                IsCashDesk = request.IsCashDesk,
                CashDeskId = request.CashDeskId,
                BranchId = request.BranchId,
                MacAddress = request.MacAddress,
                ForceLogin = false,
                IsNewSession = request.IsNewSession
            };

            return await _loginHandler.Handle(command);
        }

        public async Task<LoginResponseDto> ForceLoginAsync(LoginRequestDto request)
        {
            var command = new LoginCommand
            {
                UserId = SanitizeInput(request.UserId),
                Password = request.Password,
                IpAddress = request.IpAddress,
                IsCashDesk = request.IsCashDesk,
                CashDeskId = request.CashDeskId,
                BranchId = request.BranchId,
                MacAddress = request.MacAddress,
                ForceLogin = true,
                IsNewSession = request.IsNewSession
            };

            return await _loginHandler.Handle(command);
        }

        public async Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordRequestDto request)
        {
            var command = new ChangePasswordCommand
            {
                UserId = SanitizeInput(request.UserId),
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword,
                ConfirmPassword = request.ConfirmPassword
            };

            return await _changePasswordHandler.Handle(command);
        }

        public async Task<ValidateUserResponseDto> ValidateUserAsync(ValidateUserRequestDto request)
        {
            try
            {
                var sanitizedUserId = SanitizeInput(request.UserId);

                // Check if user exists
                var userExists = await _unitOfWork.UserRepository.Exists(sanitizedUserId);
                if (!userExists)
                {
                    return new ValidateUserResponseDto
                    {
                        IsValid = false,
                        Message = "User not found."
                    };
                }

                // Check if user is blocked
                var isBlocked = await _unitOfWork.UserRepository.IsUserBlockedAsync(sanitizedUserId);
                if (isBlocked)
                {
                    return new ValidateUserResponseDto
                    {
                        IsValid = false,
                        IsBlocked = true,
                        Message = "User is blocked."
                    };
                }

                // Check if user is enabled
                var isEnabled = await _unitOfWork.UserRepository.IsUserEnabledAsync(sanitizedUserId);
                if (!isEnabled)
                {
                    return new ValidateUserResponseDto
                    {
                        IsValid = false,
                        Message = "User is not enabled."
                    };
                }

                // Validate credentials
                var areCredentialsValid = await _unitOfWork.UserRepository.ValidateCredentialsAsync(
                    sanitizedUserId, 
                    request.Password);

                if (!areCredentialsValid)
                {
                    return new ValidateUserResponseDto
                    {
                        IsValid = false,
                        Message = "Invalid credentials."
                    };
                }

                // Check if password change is required
                var user = await _unitOfWork.UserRepository.GetByIdAsync(sanitizedUserId);
                var requiresPasswordChange = user?.UsrChgPas ?? false;

                return new ValidateUserResponseDto
                {
                    IsValid = true,
                    IsBlocked = false,
                    RequiresPasswordChange = requiresPasswordChange,
                    Message = "User validated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ValidateUserResponseDto
                {
                    IsValid = false,
                    Message = $"Validation error: {ex.Message}"
                };
            }
        }

        public async Task<string?> WhoIsLoggedAsync(string ipAddress)
        {
            try
            {
                var session = await _unitOfWork.UserSessionRepository.GetActiveSessionByIpAsync(ipAddress);
                return session?.UsrId;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            try
            {
                var result = await _unitOfWork.UserSessionRepository.TerminateSessionAsync(userId);
                await _unitOfWork.Complete();
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Sanitizes user input to prevent SQL injection and XSS attacks
        /// Similar to HttpUtility.HtmlEncode in the original implementation
        /// </summary>
        private string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove dangerous characters
            var sanitized = input.Trim();
            
            // Basic sanitization - in production, use a proper library
            sanitized = sanitized.Replace("'", "")
                                 .Replace("\"", "")
                                 .Replace("<", "")
                                 .Replace(">", "")
                                 .Replace("&", "")
                                 .Replace(";", "")
                                 .Replace("--", "")
                                 .Replace("/*", "")
                                 .Replace("*/", "");

            return sanitized;
        }
    }
}
