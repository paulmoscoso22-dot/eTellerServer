using eTeller.Application.Contracts;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.InsertFunzioneSchedule
{
    public class InsertFunzioneScheduleCommandHandler : IRequestHandler<InsertFunzioneScheduleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InsertFunzioneScheduleCommandHandler> _logger;

        public InsertFunzioneScheduleCommandHandler(IUnitOfWork unitOfWork, ILogger<InsertFunzioneScheduleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(InsertFunzioneScheduleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for FutId={FutId}", nameof(InsertFunzioneScheduleCommand), request.FutId);

            var existing = await _unitOfWork.Repository<FUNZIONISCEDULE>()
                .GetAsync(f => f.FutId == request.FutId);
            if (existing.Any())
                throw new InvalidOperationException($"Funzione con ID '{request.FutId}' già esistente.");

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

            var entity = new FUNZIONISCEDULE
            {
                FutId = request.FutId,
                FutDes = request.FutDes,
                FutFunname = request.FutFunname,
                FutScriptname = request.FutScriptname,
                FutTimeout = request.FutTimeout,
                FutActive = request.FutActive,
                FutOffline = request.FutOffline,
                FutTrace = request.FutTrace,
                FutAutatt = request.FutAutatt,
                FutHosval = request.FutHosval,
                FutPeriodtyp = request.FutPeriodtyp,
                FutPeriod = request.FutPeriod,
                FutStart = request.FutStart,
                FutEnd = request.FutEnd,
                FutNamedll = request.FutNamedll,
                FutClassname = request.FutClassname,
                FutErrcount = request.FutErrcount,
                FutOnetimerun = false,
                FutLoop = false,
                FutDatins = DateTime.Now,
                FutDatmod = DateTime.Now
            };

            _unitOfWork.Repository<FUNZIONISCEDULE>().AddEntity(entity);
            var saved = await _unitOfWork.Complete();
            if (saved == 0)
                throw new InvalidOperationException($"Impossibile salvare la funzione '{request.FutId}'.");

            await _unitOfWork.TraceRepository.InsertTrace(
                traTime: DateTime.Now,
                traUser: request.TraUser,
                traFunCode: "OPE",
                traSubFun: "INSERT_FUNZIONISCHEDULE",
                traStation: request.TraStation,
                traTabNam: "FUNZIONISHEDULE",
                traEntCode: request.FutId,
                traRevTrxTrace: null,
                traDes: $"INSERT: ID={request.FutId} DES={request.FutDes} FUNNAME={request.FutFunname} AUTATT={request.FutAutatt}",
                traExtRef: null,
                traError: false);

            _logger.LogInformation("Handled {CommandName}, inserted FutId={FutId}", nameof(InsertFunzioneScheduleCommand), request.FutId);
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
