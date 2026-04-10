using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Users.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(IUnitOfWork unitOfWork, ILogger<ResetPasswordCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName}", nameof(ResetPasswordCommand));
            var result = await _unitOfWork.ManagerRepository.ResetPasswordAsync(
                request.UsrId,
                request.ChgPas,
                request.UsrPass,
                cancellationToken);

            if (result > 0)
                _logger.LogWarning("Password reset failed for user {UserId}", request.UsrId);
            _logger.LogInformation("Handled {CommandName}, rows affected: {Result}", nameof(ResetPasswordCommand), result);
            return result;
        }
    }
}