using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.UpdateSpAntirecAppearer
{
    public class UpdateSpAntirecAppearerCommandHandler : IRequestHandler<UpdateSpAntirecAppearerCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateSpAntirecAppearerCommandHandler> _logger;

        public UpdateSpAntirecAppearerCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateSpAntirecAppearerCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(UpdateSpAntirecAppearerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for AraId={AraId}, AraName={AraName}",
                nameof(UpdateSpAntirecAppearerCommand), request.AraId, request.AraName);

            var araId = await _unitOfWork.VigilanzaSpRepository.UpdateAntirecAppearer(
                request.AraId,
                request.AraRecdate,
                request.AraName,
                request.AraBirthdate,
                request.AraBirthplace,
                request.AraNationality,
                request.AraIddocnum,
                request.AraDocexpdate,
                request.AraRepresents,
                request.AraAddress,
                request.AraRecComplete,
                request.AraIsupdated
            );

            _logger.LogInformation("Handled {CommandName}, updated AraId={AraId}", nameof(UpdateSpAntirecAppearerCommand), araId);
            return araId;
        }
    }
}
