using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Manager.Commands.DeleteSysFunction
{
    public class DeleteSysFunctionCommandHandler : IRequestHandler<DeleteSysFunctionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteSysFunctionCommandHandler> _logger;

        public DeleteSysFunctionCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteSysFunctionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteSysFunctionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for User={TraUser}, FunId={FunId}", nameof(DeleteSysFunctionCommand), request.TraUser, request.FunId);

            try
            {
                var result = await _unitOfWork.ManagerSpRepository.DeleteSysFunctionAsync(
                    request.TraUser,
                    request.TraStation,
                    request.FunId,
                    cancellationToken);

                _logger.LogInformation("Handled {CommandName}, Success={Success}", nameof(DeleteSysFunctionCommand), result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {CommandName} for User={TraUser}, FunId={FunId}", nameof(DeleteSysFunctionCommand), request.TraUser, request.FunId);
                throw;
            }
        }
    }
}
