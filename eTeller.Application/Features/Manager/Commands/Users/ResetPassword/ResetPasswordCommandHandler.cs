using eTeller.Application.Contracts;
using eTeller.Application.Shared;
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
            try 
            {
                if (!Utility.VerifyPassword(request.UsrPass, request.UsrPass2))

                {
                    _logger.LogWarning("Password verification failed for user {UserId}", request.UsrId);
                    throw new InvalidOperationException("Password verification failed");
                }

                await _unitOfWork.BeginTransactionAsync();
                _logger.LogInformation("Handling {CommandName}", nameof(ResetPasswordCommand));
                var result = await _unitOfWork.ManagerRepository.ResetPasswordAsync(
                    request.UsrId,
                    request.ChgPas,
                    request.UsrPass,
                    cancellationToken);

                if (result < 0)
                {
                    await _unitOfWork.TraceRepository.InsertTrace(
                        traTime: DateTime.Now,
                        traUser: "User",
                        traFunCode: "OPE",
                        traSubFun: "ResetPassword",
                        traStation: "SERVER",
                        traTabNam: "sys_User",
                        traEntCode: request.UsrId.ToString(),
                        traRevTrxTrace: null,
                        traDes: $"reset password Ok",
                        traExtRef: null,
                        traError: false
                    );
                    _logger.LogWarning("Password reset failed for user {UserId}", request.UsrId);
                }
                if (result >= 0)
                {
                    await _unitOfWork.TraceRepository.InsertTrace(
                       traTime: DateTime.Now,
                       traUser: "User",
                       traFunCode: "OPE",
                       traSubFun: "ResetPassword",
                       traStation: "SERVER",
                       traTabNam: "sys_User",
                       traEntCode: request.UsrId.ToString(),
                       traRevTrxTrace: null,
                       traDes: $"reset password not Ok",
                       traExtRef: null,
                       traError: false
                   );
                }
                await _unitOfWork.Complete();
                await _unitOfWork.CommitAsync();
                _logger.LogInformation("Handled {CommandName}, rows affected: {Result}", nameof(ResetPasswordCommand), result);
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.Rollback();
                _logger.LogError(ex, "An error occurred while handling {CommandName} for user {UserId}", nameof(ResetPasswordCommand), request.UsrId);
                throw;
            }
        }
    }
}