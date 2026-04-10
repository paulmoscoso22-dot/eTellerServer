using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.InsertSpAntirecAppearer
{
    public class InsertSpAntirecAppearerCommandHandler : IRequestHandler<InsertSpAntirecAppearerCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InsertSpAntirecAppearerCommandHandler> _logger;

        public InsertSpAntirecAppearerCommandHandler(IUnitOfWork unitOfWork, ILogger<InsertSpAntirecAppearerCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(InsertSpAntirecAppearerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for AraId={AraId}, AraName={AraName}",
                nameof(InsertSpAntirecAppearerCommand), request.AraId, request.AraName);

            var hisId = await _unitOfWork.VigilanzaRepository.InsertHisAntirecAppearer(
                request.HisDate,
                request.AraId,
                request.AraRecdate,
                request.AraName,
                request.AraBirthdate,
                request.AraBirthplace,
                request.AraNationality,
                request.AraIddocnum,
                request.AraDocexpdate,
                request.AraRecComplete,
                request.AraRepresents,
                request.AraAddress
            );

            _logger.LogInformation("Handled {CommandName}, returned HIS_ID={HisId}", nameof(InsertSpAntirecAppearerCommand), hisId);
            return hisId;
        }
    }
}
