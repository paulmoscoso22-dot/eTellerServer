using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Client;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.CassePeriferiche.Casse.InsertClient
{
    public class InsertClientCommandHandler : IRequestHandler<InsertClientCommand, ClientVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<InsertClientCommandHandler> _logger;

        public InsertClientCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<InsertClientCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ClientVm> Handle(InsertClientCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for CliId={CliId}", nameof(InsertClientCommand), request.CliId);

            var existingId = await _unitOfWork.Repository<Domain.Models.Client>()
                .GetAsync(c => c.CliId == request.CliId);
            if (existingId.Any())
                throw new InvalidOperationException($"La cassa con ID '{request.CliId}' esiste già.");

            var existingIp = await _unitOfWork.Repository<Domain.Models.Client>()
                .GetAsync(c => c.CliIp == request.CliIp);
            if (existingIp.Any())
                throw new InvalidOperationException($"L'indirizzo IP '{request.CliIp}' è già assegnato a un'altra cassa.");

            var existingMac = await _unitOfWork.Repository<Domain.Models.Client>()
                .GetAsync(c => c.CliMac == request.CliMac);
            if (existingMac.Any())
                throw new InvalidOperationException($"L'indirizzo MAC '{request.CliMac}' è già assegnato a un'altra cassa.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var entity = new Domain.Models.Client
                {
                    CliId       = request.CliId,
                    CliIp       = request.CliIp,
                    CliMac      = request.CliMac,
                    CliAuthcode = request.CliAuthcode,
                    CliBraId    = request.CliBraId,
                    CliStatus   = request.CliStatus,
                    CliLingua   = request.CliLingua,
                    CliDes      = request.CliDes,
                    CliOff      = request.CliOff,
                    CliCnt      = 0,
                    CliDatcounter = null
                };

                _unitOfWork.Repository<Domain.Models.Client>().AddEntity(entity);

                if (request.DeviceIds != null && request.DeviceIds.Length > 0)
                {
                    var assignments = request.DeviceIds.Select(devId => new Domain.Models.ClientDevice
                    {
                        CliId = request.CliId,
                        DevId = devId
                    }).ToList();
                    _unitOfWork.Repository<Domain.Models.ClientDevice>().AddRangeEntity(assignments);
                }

                var saved = await _unitOfWork.Complete();
                if (saved == 0)
                {
                    _logger.LogError("Failed to insert client {CliId}", request.CliId);
                    throw new InvalidOperationException($"Impossibile salvare la cassa '{request.CliId}'.");
                }

                await _unitOfWork.TraceRepository.InsertTrace(
                    traTime: DateTime.Now,
                    traUser: request.TraUser,
                    traFunCode: "OPE",
                    traSubFun: "INSERTCLIENT",
                    traStation: request.TraStation,
                    traTabNam: "sys_CLIENT",
                    traEntCode: request.CliId,
                    traRevTrxTrace: null,
                    traDes: $"INSERT CLIENT: ID={request.CliId} IP={request.CliIp} MAC={request.CliMac} BRA={request.CliBraId} STATUS={request.CliStatus} DEVICES={string.Join(",", request.DeviceIds ?? [])}",
                    traExtRef: null,
                    traError: false);

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Handled {CommandName}, inserted client {CliId}", nameof(InsertClientCommand), request.CliId);
                return _mapper.Map<ClientVm>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {CommandName} for CliId={CliId}", nameof(InsertClientCommand), request.CliId);
                await _unitOfWork.Rollback();
                throw new InvalidOperationException($"Errore durante l'inserimento della cassa: {ex.Message}");
            }
        }
    }
}
