using eTeller.Application.Contracts;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.InsertServizio
{
    public class InsertServizioCommandHandler : IRequestHandler<InsertServizioCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InsertServizioCommandHandler> _logger;

        public InsertServizioCommandHandler(IUnitOfWork unitOfWork, ILogger<InsertServizioCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(InsertServizioCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for SerId={SerId}", nameof(InsertServizioCommand), request.SerId);

            var existing = await _unitOfWork.Repository<SERVIZI>().GetAsync(s => s.SerId == request.SerId);
            if (existing.Any())
                throw new InvalidOperationException($"Servizio con ID '{request.SerId}' già esistente.");

            var entity = new SERVIZI
            {
                SerId = request.SerId,
                SerDes = request.SerDes,
                SerDeserr = string.Empty,
                SerTrace = request.SerTrace,
                SerEmail = request.SerEmail,
                SerSyserrmail = request.SerSyserrmail,
                SerApperrmail = request.SerApperrmail,
                SerEnable = request.SerEnable,
                SerRunning = false,
                SerLastrun = null
            };

            _unitOfWork.Repository<SERVIZI>().AddEntity(entity);
            var saved = await _unitOfWork.Complete();
            if (saved == 0)
                throw new InvalidOperationException($"Impossibile salvare il servizio '{request.SerId}'.");

            await _unitOfWork.TraceRepository.InsertTrace(
                traTime: DateTime.Now,
                traUser: request.TraUser,
                traFunCode: "OPE",
                traSubFun: "INSERT_SERVIZIO",
                traStation: request.TraStation,
                traTabNam: "SERVIZI",
                traEntCode: request.SerId,
                traRevTrxTrace: null,
                traDes: $"INSERT: ID={request.SerId} DES={request.SerDes}",
                traExtRef: null,
                traError: false);

            _logger.LogInformation("Handled {CommandName}, inserted SerId={SerId}", nameof(InsertServizioCommand), request.SerId);
            return true;
        }
    }
}
