using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.DeleteARA
{
    public class DeleteARACommandHandler : IRequestHandler<DeleteARACommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteARACommandHandler> _logger;

        public DeleteARACommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteARACommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteARACommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Handling {CommandName} for User={TraUser}, AraId={AraId}",
                nameof(DeleteARACommand), request.TraUser, request.AraId);

            try
            {
                var result = await _unitOfWork.VigilanzaRepository.DeleteARA(
                    request.TraUser,
                    request.TraStation,
                    request.AraId
                );

                _logger.LogInformation(
                    "Handled {CommandName}, Success={Success}", 
                    nameof(DeleteARACommand), result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error handling {CommandName} for User={TraUser}, AraId={AraId}", 
                    nameof(DeleteARACommand), request.TraUser, request.AraId);
                throw;
            }
        }
    }
}
