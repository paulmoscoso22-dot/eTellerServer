using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.ScheduleOneTimeTask
{
    public class ScheduleOneTimeTaskCommandHandler : IRequestHandler<ScheduleOneTimeTaskCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ScheduleOneTimeTaskCommandHandler> _logger;

        public ScheduleOneTimeTaskCommandHandler(IUnitOfWork unitOfWork, ILogger<ScheduleOneTimeTaskCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(ScheduleOneTimeTaskCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for FutId={FutId}", nameof(ScheduleOneTimeTaskCommand), request.FutId);

            var results = await _unitOfWork.Repository<FUNZIONISCEDULE>()
                .GetAsync(f => f.FutId == request.FutId);
            var entity = results.FirstOrDefault()
                ?? throw new NotFoundException(nameof(FUNZIONISCEDULE), request.FutId);

            if (entity.FutAutatt != true)
                throw new InvalidOperationException($"La funzione '{request.FutId}' non è auto-schedulata: OneTimeRun non applicabile.");

            entity.FutOnetimerun = true;
            entity.FutDatmod = DateTime.Now;

            _unitOfWork.Repository<FUNZIONISCEDULE>().UpdateEntity(entity);
            var saved = await _unitOfWork.Complete();
            if (saved == 0)
                throw new InvalidOperationException($"Impossibile impostare OneTimeRun per la funzione '{request.FutId}'.");

            await _unitOfWork.TraceRepository.InsertTrace(
                traTime: DateTime.Now,
                traUser: request.TraUser,
                traFunCode: "MOD",
                traSubFun: "ONETIMERUN_FUNZIONISCHEDULE",
                traStation: request.TraStation,
                traTabNam: "FUNZIONISHEDULE",
                traEntCode: request.FutId,
                traRevTrxTrace: null,
                traDes: $"ONE TIME RUN: ID={request.FutId}",
                traExtRef: null,
                traError: false);

            _logger.LogInformation("Handled {CommandName}, set OneTimeRun for FutId={FutId}", nameof(ScheduleOneTimeTaskCommand), request.FutId);
            return true;
        }
    }
}
