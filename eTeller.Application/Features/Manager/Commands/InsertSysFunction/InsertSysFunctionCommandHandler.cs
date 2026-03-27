using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Manager.Commands.InsertSysFunction
{
    public class InsertSysFunctionCommandHandler : IRequestHandler<InsertSysFunctionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InsertSysFunctionCommandHandler> _logger;

        public InsertSysFunctionCommandHandler(IUnitOfWork unitOfWork, ILogger<InsertSysFunctionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(InsertSysFunctionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for User={TraUser}, FunName={FunName}", nameof(InsertSysFunctionCommand), request.TraUser, request.FunName);

            try
            {
                var result = await _unitOfWork.ManagerSpRepository.InsertSysFunctionAsync(
                    request.TraUser,
                    request.TraStation,
                    request.FunName,
                    request.FunDescription,
                    request.FunHostcode,
                    request.Offline,
                    cancellationToken);

                _logger.LogInformation("Handled {CommandName}, Success={Success}", nameof(InsertSysFunctionCommand), result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {CommandName} for User={TraUser}, FunName={FunName}", nameof(InsertSysFunctionCommand), request.TraUser, request.FunName);
                throw;
            }
        }
    }
}
