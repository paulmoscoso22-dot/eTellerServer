using eTeller.Application.Contracts;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.UpdateBookingRc
{
    public class UpdateBookingRcCommandHandler : IRequestHandler<UpdateBookingRcCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateBookingRcCommandHandler> _logger;

        public UpdateBookingRcCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateBookingRcCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateBookingRcCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for CutId={CutId} OptId={OptId} ActId={ActId}",
                nameof(UpdateBookingRcCommand), request.BrcCutId, request.BrcOptId, request.BrcActId);

            var entity = new StBookingRc
            {
                BrcCutId = request.BrcCutId,
                BrcOptId = request.BrcOptId,
                BrcActId = request.BrcActId,
                BrcCodcau = request.BrcCodcau,
                BrcCodcausto = request.BrcCodcausto,
                BrcText1 = request.BrcText1,
                BrcText2 = request.BrcText2
            };

            _unitOfWork.Repository<StBookingRc>().UpdateEntity(entity);
            await _unitOfWork.Complete();

            await _unitOfWork.TraceRepository.InsertTrace(
                traTime: DateTime.Now,
                traUser: request.TraUser,
                traFunCode: "OPE",
                traSubFun: "UPDATE_BOOKINRC",
                traStation: request.TraStation,
                traTabNam: "ST_BOOKING_RC",
                traEntCode: $"{request.BrcCutId}/{request.BrcOptId}/{request.BrcActId}",
                traRevTrxTrace: null,
                traDes: $"UPDATE: CUT={request.BrcCutId} OPT={request.BrcOptId} ACT={request.BrcActId} CODCAU={request.BrcCodcau} CODCAUSTO={request.BrcCodcausto}",
                traExtRef: null,
                traError: false);

            _logger.LogInformation("Handled {CommandName}, updated BookingRc CutId={CutId} OptId={OptId} ActId={ActId}",
                nameof(UpdateBookingRcCommand), request.BrcCutId, request.BrcOptId, request.BrcActId);

            return true;
        }
    }
}
