using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Functions.UpdateSysFunction
{
    public class UpdateSysFunctionCommandHandler : IRequestHandler<UpdateSysFunctionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateSysFunctionCommandHandler> _logger;

        public UpdateSysFunctionCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateSysFunctionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateSysFunctionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for User={TraUser}, FunId={FunId}", nameof(UpdateSysFunctionCommand), request.TraUser, request.FunId);

            var result = await _unitOfWork.ManagerRepository.UpdateSysFunctionAsync(
                request.TraUser,
                request.TraStation,
                request.FunId,
                request.FunName,
                request.FunDescription,
                request.FunHostcode,
                request.Offline,
                cancellationToken);

            if (!result)
                _logger.LogWarning("Failed to update function with FunId={FunId}", request.FunId);

            _logger.LogInformation("Handled {CommandName}, Success={Success}", nameof(UpdateSysFunctionCommand), result);

            return result;
        }
    }
}
