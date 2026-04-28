using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.ResetFunctionError
{
    public class ResetFunctionErrorCommandHandler : IRequestHandler<ResetFunctionErrorCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ResetFunctionErrorCommandHandler> _logger;

        public ResetFunctionErrorCommandHandler(IUnitOfWork unitOfWork, ILogger<ResetFunctionErrorCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(ResetFunctionErrorCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for FutId={FutId}", nameof(ResetFunctionErrorCommand), request.FutId);

            var results = await _unitOfWork.Repository<FUNZIONISCEDULE>()
                .GetAsync(f => f.FutId == request.FutId);
            var entity = results.FirstOrDefault()
                ?? throw new NotFoundException(nameof(FUNZIONISCEDULE), request.FutId);

            entity.FutErrcount = 0;
            entity.FutLastrunok = true;
            entity.FutDatmod = DateTime.Now;

            _unitOfWork.Repository<FUNZIONISCEDULE>().UpdateEntity(entity);
            var saved = await _unitOfWork.Complete();
            if (saved == 0)
                throw new InvalidOperationException($"Impossibile resettare gli errori per la funzione '{request.FutId}'.");

            await _unitOfWork.TraceRepository.InsertTrace(
                traTime: DateTime.Now,
                traUser: request.TraUser,
                traFunCode: "MOD",
                traSubFun: "RESET_ERROR_FUNZIONISCHEDULE",
                traStation: request.TraStation,
                traTabNam: "FUNZIONISHEDULE",
                traEntCode: request.FutId,
                traRevTrxTrace: null,
                traDes: $"RESET ERROR: ID={request.FutId}",
                traExtRef: null,
                traError: false);

            _logger.LogInformation("Handled {CommandName}, reset errors for FutId={FutId}", nameof(ResetFunctionErrorCommand), request.FutId);
            return true;
        }
    }
}
