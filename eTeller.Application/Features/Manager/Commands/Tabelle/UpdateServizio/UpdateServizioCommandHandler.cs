using eTeller.Application.Contracts;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.UpdateServizio
{
    public class UpdateServizioCommandHandler : IRequestHandler<UpdateServizioCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateServizioCommandHandler> _logger;

        public UpdateServizioCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateServizioCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateServizioCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for SerId={SerId}", nameof(UpdateServizioCommand), request.SerId);

            var existing = await _unitOfWork.Repository<SERVIZI>().GetAsync(s => s.SerId == request.SerId);
            var entity = existing.FirstOrDefault()
                ?? throw new InvalidOperationException($"Servizio con ID '{request.SerId}' non trovato.");

            entity.SerDes = request.SerDes;
            entity.SerTrace = request.SerTrace;
            entity.SerEmail = request.SerEmail;
            entity.SerSyserrmail = request.SerSyserrmail;
            entity.SerApperrmail = request.SerApperrmail;
            entity.SerEnable = request.SerEnable;

            _unitOfWork.Repository<SERVIZI>().UpdateEntity(entity);
            var saved = await _unitOfWork.Complete();
            if (saved == 0)
                throw new InvalidOperationException($"Impossibile aggiornare il servizio '{request.SerId}'.");

            await _unitOfWork.TraceRepository.InsertTrace(
                traTime: DateTime.Now,
                traUser: request.TraUser,
                traFunCode: "OPE",
                traSubFun: "UPDATE_SERVIZIO",
                traStation: request.TraStation,
                traTabNam: "SERVIZI",
                traEntCode: request.SerId,
                traRevTrxTrace: null,
                traDes: $"UPDATE: ID={request.SerId} DES={request.SerDes}",
                traExtRef: null,
                traError: false);

            _logger.LogInformation("Handled {CommandName}, updated SerId={SerId}", nameof(UpdateServizioCommand), request.SerId);
            return true;
        }
    }
}
