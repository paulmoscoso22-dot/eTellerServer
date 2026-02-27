using eTeller.Application.Contracts;
using eTeller.Application.Features.User.Commands;
using eTeller.Application.Features.User.DTOs;
using eTeller.Domain.Common;
using eTeller.Domain.Exceptions;

namespace eTeller.Application.Features.User.Handlers
{
    public class LoginCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand command)
        {
            try
            {
                // Variables matching legacy WebFormLogin2 system
                // IP - Client IP address
                string ipAddress = command.IpAddress;
                
                // Handle IPv6 localhost - convert to specific IP for development
                
                // NuovaApertura - New session flag
                bool nuovaApertura = command.IsNewSession;
                
                // CLI_ID - Cash desk ID
                string cliId = string.Empty;
                
                // BRA_ID - Branch ID
                string braId = command.BranchId ?? string.Empty;
                
                // CLI_STATUS - Cash desk status (ENABLED/DISABLED)
                string cliStatus = string.Empty;
                
                // CLI_LINGUA - Cash desk language
                string cliLingua = string.Empty;
                
                // macisok - MAC address validation flag
                bool macIsOk = true;
                
                // isCassa - Is this a cash desk station?
                bool isCassa = !string.IsNullOrEmpty(command.CashDeskId);
                
                // canUseTeller - Can user access teller functions?
                bool canUseTeller = false;

                // Get client information by IP address
                var client = await _unitOfWork.ClientRepository.WhoIsLogged(ipAddress);
                if (client != null)
                {
                    cliId = client.CliId;
                    braId = client.CliBraId;
                    cliStatus = client.CliStatus;
                    cliLingua = client.CliLingua ?? string.Empty;
                    isCassa = true;
                }

                // Note: Cash desk validation is disabled as sys_CLIENTS table doesn't exist
                // To enable, create the table and uncomment the validation logic below
                
                /* CASH DESK VALIDATION - Uncomment when sys_CLIENTS table exists
                // Step 1: Check if IP corresponds to a cash desk
                var cashDesk = await _unitOfWork.CashDeskRepository.GetByIpAddressAsync(ipAddress);
                isCassa = cashDesk != null;

                if (isCassa && cashDesk != null)
                {
                    cliId = cashDesk.CliId;
                    braId = cashDesk.CliBraId;
                    cliStatus = cashDesk.CliStatus;
                    cliLingua = cashDesk.CliLingua ?? string.Empty;

                    // Validate cash desk status
                    if (cliStatus != CashDeskStatusConstants.Enabled)
                    {
                        return new LoginResponseDto
                        {
                            IsSuccessful = false,
                            ResultCode = ErrorCodes.CashDeskNotEnabled,
                            Message = "Cash desk is not enabled."
                        };
                    }

                    // Validate MAC address if provided
                    if (!string.IsNullOrEmpty(command.MacAddress))
                    {
                        macIsOk = await _unitOfWork.CashDeskRepository.ValidateMacAddressAsync(
                            cliId, 
                            command.MacAddress);

                        if (!macIsOk)
                        {
                            return new LoginResponseDto
                            {
                                IsSuccessful = false,
                                ResultCode = ErrorCodes.MacAddressError,
                                Message = "MAC address validation failed."
                            };
                        }
                    }

                    // Validate printers fiche
                    var printerCount = await _unitOfWork.CashDeskRepository.CountPrintersFicheAsync(cliId);
                    if (printerCount == 0)
                    {
                        return new LoginResponseDto
                        {
                            IsSuccessful = false,
                            ResultCode = ErrorCodes.NoPrinterFiche,
                            Message = "No printer fiche configured for this cash desk."
                        };
                    }
                    else if (printerCount > 1)
                    {
                        return new LoginResponseDto
                        {
                            IsSuccessful = false,
                            ResultCode = ErrorCodes.MultiplePrintersFiche,
                            Message = "Multiple printers fiche configured for this cash desk."
                        };
                    }

                    canUseTeller = true;
                }
                */

                // Step 1: Check if user exists
                var userExists = await _unitOfWork.UserRepository.Exists(command.UserId);
                if (!userExists)
                {
                    return new LoginResponseDto
                    {
                        IsSuccessful = false,
                        ResultCode = ErrorCodes.UserNotExists,
                        Message = "User not found."
                    };
                }

                // Step 2: Check if user is blocked
                var isBlocked = await _unitOfWork.UserRepository.IsUserBlockedAsync(command.UserId);
                if (isBlocked)
                {
                    return new LoginResponseDto
                    {
                        IsSuccessful = false,
                        ResultCode = ErrorCodes.UserBlocked,
                        Message = "User is blocked. Please contact system administrator."
                    };
                }

                // Step 3: Check if user is enabled
                var isEnabled = await _unitOfWork.UserRepository.IsUserEnabledAsync(command.UserId);
                if (!isEnabled)
                {
                    return new LoginResponseDto
                    {
                        IsSuccessful = false,
                        ResultCode = ErrorCodes.UserNotEnabled,
                        Message = "User account is not enabled."
                    };
                }

                // Step 4: Validate credentials
                var areCredentialsValid = await _unitOfWork.UserRepository.ValidateCredentialsAsync(
                    command.UserId, 
                    command.Password);

                if (!areCredentialsValid)
                {
                    // Increment failed attempts
                    var failedAttempts = await _unitOfWork.UserRepository.IncrementFailedAttemptsAsync(command.UserId);
                    await _unitOfWork.Complete();

                    if (failedAttempts >= PasswordPolicyConstants.MaxFailedAttempts)
                    {
                        return new LoginResponseDto
                        {
                            IsSuccessful = false,
                            ResultCode = ErrorCodes.UserBlocked,
                            Message = "User has been blocked due to too many failed login attempts."
                        };
                    }

                    return new LoginResponseDto
                    {
                        IsSuccessful = false,
                        ResultCode = ErrorCodes.InvalidCredentials,
                        Message = $"Invalid credentials. {PasswordPolicyConstants.MaxFailedAttempts - failedAttempts} attempts remaining."
                    };
                }

                // Step 5: Get user details
                var user = await _unitOfWork.UserRepository.GetByIdAsync(command.UserId);
                if (user == null)
                {
                    return new LoginResponseDto
                    {
                        IsSuccessful = false,
                        ResultCode = ErrorCodes.UserNotExists,
                        Message = "User not found."
                    };
                }

                /* BRANCH VALIDATION - Uncomment when cash desk functionality is enabled
                // Step 6: Validate branch if cash desk
                if (isCassa && !string.IsNullOrEmpty(braId))
                {
                    if (user.UsrBraId != braId)
                    {
                        return new LoginResponseDto
                        {
                            IsSuccessful = false,
                            ResultCode = ErrorCodes.BranchMismatch,
                            Message = "User branch does not match cash desk branch."
                        };
                    }
                }
                */

                // Step 6: Check if password change is required
                if (user.UsrChgPas && !command.ForceLogin)
                {
                    return new LoginResponseDto
                    {
                        IsSuccessful = false,
                        ResultCode = ErrorCodes.PasswordExpired,
                        RequiresPasswordChange = true,
                        Message = "Password change is required before logging in."
                    };
                }

                // Step 7: Check if user is already logged
                var existingSession = await _unitOfWork.UserSessionRepository.GetActiveSessionByUserIdAsync(command.UserId);
                if (existingSession != null && !command.ForceLogin && nuovaApertura)
                {
                    return new LoginResponseDto
                    {
                        IsSuccessful = false,
                        ResultCode = ErrorCodes.UserAlreadyLogged,
                        UserAlreadyLogged = true,
                        ExistingLoginDate = existingSession.LoginTime,
                        Message = "User is already logged in. Use force login to disconnect the existing session."
                    };
                }

                /* CASH DESK OCCUPIED CHECK - Uncomment when cash desk functionality is enabled
                // Step 8: Check if cash desk is occupied by another user
                if (isCassa && !string.IsNullOrEmpty(cliId) && !command.ForceLogin)
                {
                    var sessionOnCashDesk = await _unitOfWork.UserSessionRepository.GetActiveSessionByIpAsync(ipAddress);
                    if (sessionOnCashDesk != null && sessionOnCashDesk.UsrId != command.UserId)
                    {
                        return new LoginResponseDto
                        {
                            IsSuccessful = false,
                            ResultCode = ErrorCodes.CashDeskOccupied,
                            Message = $"Cash desk is occupied by user: {sessionOnCashDesk.UsrId}"
                        };
                    }
                }
                */

                // Step 8: Reset failed attempts on successful login
                await _unitOfWork.UserRepository.ResetFailedAttemptsAsync(command.UserId);

                // Step 9: Create session
                var session = await _unitOfWork.UserSessionRepository.CreateSessionAsync(
                    command.UserId, 
                    string.IsNullOrEmpty(cliId) ? null : cliId, 
                    ipAddress, 
                    command.ForceLogin);
                
                await _unitOfWork.Complete();

                // Step 10: Create session DTO
                var sessionDto = new UserSessionDto
                {
                    UserId = user.UsrId,
                    UserName = user.UsrExtref ?? user.UsrId,
                    BranchId = user.UsrBraId,
                    Language = user.UsrLingua,
                    CanUseTeller = canUseTeller,
                    CashDeskId = string.IsNullOrEmpty(cliId) ? null : cliId,
                    CashDeskStatus = string.IsNullOrEmpty(cliStatus) ? null : cliStatus,
                    CashDeskLanguage = string.IsNullOrEmpty(cliLingua) ? null : cliLingua,
                    CashDeskDescription = null, // TODO: Add when CashDesk table is available
                    IpAddress = ipAddress,
                    SessionStartTime = session?.LoginTime ?? DateTime.Now,
                    SessionExpirationTime = DateTime.Now.AddHours(10)
                };

                return new LoginResponseDto
                {
                    IsSuccessful = true,
                    ResultCode = AuthenticationResultConstants.Success,
                    UserSession = sessionDto,
                    Message = "Login successful."
                };
            }
            catch (Exception ex)
            {
                return new LoginResponseDto
                {
                    IsSuccessful = false,
                    ResultCode = ErrorCodes.GenericLoginError,
                    Message = $"An error occurred during login: {ex.Message}"
                };
            }
        }
    }
}
