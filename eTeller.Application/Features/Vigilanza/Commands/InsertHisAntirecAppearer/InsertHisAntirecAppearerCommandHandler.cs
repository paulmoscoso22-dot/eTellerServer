using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.InsertHisAntirecAppearer
{
    public class InsertHisAntirecAppearerCommandHandler : IRequestHandler<InsertHisAntirecAppearerCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InsertHisAntirecAppearerCommandHandler> _logger;

        public InsertHisAntirecAppearerCommandHandler(IUnitOfWork unitOfWork, ILogger<InsertHisAntirecAppearerCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(InsertHisAntirecAppearerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for AraId={AraId}, AraName={AraName}, HisDate={HisDate}",
                nameof(InsertHisAntirecAppearerCommand), request.AraId, request.AraName, request.HisDate);

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

            _logger.LogInformation("Handled {CommandName}, returned HIS_ID={HisId}", 
                nameof(InsertHisAntirecAppearerCommand), hisId);
            
            return hisId;
        }
    }
}
