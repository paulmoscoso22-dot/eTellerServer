
using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.UpdateFunzioneSchedule
{
    public class UpdateFunzioneScheduleCommandHandler : IRequestHandler<UpdateFunzioneScheduleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateFunzioneScheduleCommandHandler> _logger;

        public UpdateFunzioneScheduleCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateFunzioneScheduleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateFunzioneScheduleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for FutId={FutId}", nameof(UpdateFunzioneScheduleCommand), request.FutId);

            var results = await _unitOfWork.Repository<FUNZIONISCEDULE>()
                .GetAsync(f => f.FutId == request.FutId);
            var entity = results.FirstOrDefault()
                ?? throw new NotFoundException(nameof(FUNZIONISCEDULE), request.FutId);

            if (request.FutAutatt)
            {
                if (string.IsNullOrWhiteSpace(request.FutPeriodtyp))
                    throw new InvalidOperationException("FutPeriodtyp è obbligatorio per le funzioni auto-schedulate.");
                if (!request.FutPeriod.HasValue || request.FutPeriod <= 0)
                    throw new InvalidOperationException("FutPeriod deve essere > 0 per le funzioni auto-schedulate.");
                if (!string.IsNullOrWhiteSpace(request.FutStart) && !IsTimeValid(request.FutStart))
                    throw new InvalidOperationException($"FutStart '{request.FutStart}' non è un orario valido (HH:MM:SS).");
                if (!string.IsNullOrWhiteSpace(request.FutEnd) && !IsTimeValid(request.FutEnd))
                    throw new InvalidOperationException($"FutEnd '{request.FutEnd}' non è un orario valido (HH:MM:SS).");
            }

            entity.FutDes = request.FutDes;
            entity.FutFunname = request.FutFunname;
            entity.FutScriptname = request.FutScriptname;
            entity.FutTimeout = request.FutTimeout;
            entity.FutActive = request.FutActive;
            entity.FutOffline = request.FutOffline;
            entity.FutTrace = request.FutTrace;
            entity.FutAutatt = request.FutAutatt;
            entity.FutHosval = request.FutHosval;
            entity.FutPeriodtyp = request.FutPeriodtyp;
            entity.FutPeriod = request.FutPeriod;
            entity.FutStart = request.FutStart;
            entity.FutEnd = request.FutEnd;
            entity.FutNamedll = request.FutNamedll;
            entity.FutClassname = request.FutClassname;
            entity.FutErrcount = request.FutErrcount;
            entity.FutDatmod = DateTime.Now;

            _unitOfWork.Repository<FUNZIONISCEDULE>().UpdateEntity(entity);
            var saved = await _unitOfWork.Complete();
            if (saved == 0)
                throw new InvalidOperationException($"Impossibile aggiornare la funzione '{request.FutId}'.");

            await _unitOfWork.TraceRepository.InsertTrace(
                traTime: DateTime.Now,
                traUser: request.TraUser,
                traFunCode: "MOD",
                traSubFun: "UPDATE_FUNZIONISCHEDULE",
                traStation: request.TraStation,
                traTabNam: "FUNZIONISHEDULE",
                traEntCode: request.FutId,
                traRevTrxTrace: null,
                traDes: $"UPDATE: ID={request.FutId} DES={request.FutDes} FUNNAME={request.FutFunname} AUTATT={request.FutAutatt}",
                traExtRef: null,
                traError: false);

            _logger.LogInformation("Handled {CommandName}, updated FutId={FutId}", nameof(UpdateFunzioneScheduleCommand), request.FutId);
            return true;
        }

        private static bool IsTimeValid(string time)
        {
            try
            {
                var parts = time.Split(':');
                int h = int.Parse(parts[0]), m = int.Parse(parts[1]), s = int.Parse(parts[2]);
                if (h < 0 || h > 24) return false;
                if (m < 0 || m > 59) return false;
                if (s < 0 || s > 59) return false;
                if (h == 24 && (m != 0 || s != 0)) return false;
                return true;
            }
            catch { return false; }
        }
    }
}
