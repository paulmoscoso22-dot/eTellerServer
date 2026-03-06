using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.UpdateARA
{
    public class UpdateARACommandHandler : IRequestHandler<UpdateARACommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateARACommandHandler> _logger;

        public UpdateARACommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateARACommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(UpdateARACommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Handling {CommandName} for User={TraUser}, AraId={AraId}, Name={AraName}",
                nameof(UpdateARACommand), request.TraUser, request.AraId, request.AraName);

            try
            {
                var result = await _unitOfWork.VigilanzaSpRepository.UpdateARA(
                    request.TraUser,
                    request.TraStation,
                    request.AraId,
                    request.AraName,
                    request.AraBirthdate,
                    request.AraBirthplace,
                    request.AraNationality,
                    request.AraIddocnum,
                    request.AraDocexpdate,
                    request.AraRepresents,
                    request.AraAddress,
                    request.AraRecComplete
                );

                _logger.LogInformation(
                    "Handled {CommandName}, AraId={AraId}", 
                    nameof(UpdateARACommand), result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error handling {CommandName} for User={TraUser}, AraId={AraId}, Name={AraName}", 
                    nameof(UpdateARACommand), request.TraUser, request.AraId, request.AraName);
                throw;
            }
        }
    }
}
