using eTeller.Application.Contracts;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.DeleteServizio
{
    public class DeleteServizioCommandHandler : IRequestHandler<DeleteServizioCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteServizioCommandHandler> _logger;

        public DeleteServizioCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteServizioCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteServizioCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for SerId={SerId}", nameof(DeleteServizioCommand), request.SerId);

            var existing = await _unitOfWork.Repository<SERVIZI>().GetAsync(s => s.SerId == request.SerId);
            var entity = existing.FirstOrDefault()
                ?? throw new InvalidOperationException($"Servizio con ID '{request.SerId}' non trovato.");

            _unitOfWork.Repository<SERVIZI>().DeleteEntity(entity);
            var saved = await _unitOfWork.Complete();
            if (saved == 0)
                throw new InvalidOperationException($"Impossibile eliminare il servizio '{request.SerId}'.");

            await _unitOfWork.TraceRepository.InsertTrace(
                traTime: DateTime.Now,
                traUser: request.TraUser,
                traFunCode: "OPE",
                traSubFun: "DELETE_SERVIZIO",
                traStation: request.TraStation,
                traTabNam: "SERVIZI",
                traEntCode: request.SerId,
                traRevTrxTrace: null,
                traDes: $"DELETE: ID={request.SerId}",
                traExtRef: null,
                traError: false);

            _logger.LogInformation("Handled {CommandName}, deleted SerId={SerId}", nameof(DeleteServizioCommand), request.SerId);
            return true;
        }
    }
}
