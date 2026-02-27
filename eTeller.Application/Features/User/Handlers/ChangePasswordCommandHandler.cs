using eTeller.Application.Contracts;
using eTeller.Application.Features.User.Commands;
using eTeller.Application.Features.User.DTOs;
using eTeller.Domain.Common;
using eTeller.Domain.Exceptions;

namespace eTeller.Application.Features.User.Handlers
{
    public class ChangePasswordCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ChangePasswordResponseDto> Handle(ChangePasswordCommand command)
        {
            var errors = new List<string>();

            try
            {
                // Step 1: Validate new password matches confirmation
                if (command.NewPassword != command.ConfirmPassword)
                {
                    errors.Add("New password and confirmation password do not match.");
                    return new ChangePasswordResponseDto
                    {
                        IsSuccessful = false,
                        Message = "Password change failed.",
                        Errors = errors
                    };
                }

                // Step 2: Check if user exists
                var user = await _unitOfWork.UserRepository.GetByIdAsync(command.UserId);
                if (user == null)
                {
                    throw new UserNotFoundException(command.UserId);
                }

                // Step 3: Validate current password
                var isCurrentPasswordValid = await _unitOfWork.UserRepository.ValidateCredentialsAsync(
                    command.UserId, 
                    command.CurrentPassword);

                if (!isCurrentPasswordValid)
                {
                    errors.Add("Current password is incorrect.");
                    return new ChangePasswordResponseDto
                    {
                        IsSuccessful = false,
                        Message = "Password change failed.",
                        Errors = errors
                    };
                }

                // Step 4: Validate password length
                if (command.NewPassword.Length < PasswordPolicyConstants.MinimumLength)
                {
                    throw new PasswordTooShortException(PasswordPolicyConstants.MinimumLength);
                }

                // Step 5: Validate minimum digits
                var digitCount = command.NewPassword.Count(char.IsDigit);
                if (digitCount < PasswordPolicyConstants.MinimumDigits)
                {
                    throw new PasswordMinDigitException(PasswordPolicyConstants.MinimumDigits);
                }

                // Step 6: Check password history
                var isInHistory = await _unitOfWork.UserRepository.IsPasswordInHistoryAsync(
                    command.UserId, 
                    command.NewPassword, 
                    PasswordPolicyConstants.PasswordHistoryCount);

                if (isInHistory)
                {
                    throw new PasswordNotValidException("Password has been used recently. Please choose a different password.");
                }

                // Step 7: Update password
                await _unitOfWork.UserRepository.UpdatePasswordAsync(command.UserId, command.NewPassword);
                await _unitOfWork.Complete();

                return new ChangePasswordResponseDto
                {
                    IsSuccessful = true,
                    Message = "Password changed successfully."
                };
            }
            catch (PasswordTooShortException ex)
            {
                errors.Add(ex.Message);
                return new ChangePasswordResponseDto
                {
                    IsSuccessful = false,
                    Message = "Password change failed.",
                    Errors = errors
                };
            }
            catch (PasswordMinDigitException ex)
            {
                errors.Add(ex.Message);
                return new ChangePasswordResponseDto
                {
                    IsSuccessful = false,
                    Message = "Password change failed.",
                    Errors = errors
                };
            }
            catch (PasswordNotValidException ex)
            {
                errors.Add(ex.Message);
                return new ChangePasswordResponseDto
                {
                    IsSuccessful = false,
                    Message = "Password change failed.",
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                errors.Add($"An unexpected error occurred: {ex.Message}");
                return new ChangePasswordResponseDto
                {
                    IsSuccessful = false,
                    Message = "Password change failed.",
                    Errors = errors
                };
            }
        }
    }
}
