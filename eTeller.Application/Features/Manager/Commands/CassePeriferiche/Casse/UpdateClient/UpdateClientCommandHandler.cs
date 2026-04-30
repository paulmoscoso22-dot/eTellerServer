using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Client;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.CassePeriferiche.Casse.UpdateClient
{
    public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, ClientVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateClientCommandHandler> _logger;

        public UpdateClientCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateClientCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ClientVm> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for CliId={CliId}", nameof(UpdateClientCommand), request.CliId);

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var existing = (await _unitOfWork.Repository<Domain.Models.Client>()
                    .GetAsync(c => c.CliId == request.CliId)).FirstOrDefault();

                if (existing == null)
                {
                    _logger.LogWarning("Client {CliId} not found", request.CliId);
                    throw new InvalidOperationException($"La cassa '{request.CliId}' non trovata.");
                }

                var ipConflict = await _unitOfWork.Repository<Domain.Models.Client>()
                    .GetAsync(c => c.CliIp == request.CliIp && c.CliId != request.CliId);
                if (ipConflict.Any())
                    throw new InvalidOperationException($"L'indirizzo IP '{request.CliIp}' è già assegnato a un'altra cassa.");

                var macConflict = await _unitOfWork.Repository<Domain.Models.Client>()
                    .GetAsync(c => c.CliMac == request.CliMac && c.CliId != request.CliId);
                if (macConflict.Any())
                    throw new InvalidOperationException($"L'indirizzo MAC '{request.CliMac}' è già assegnato a un'altra cassa.");

                existing.CliIp     = request.CliIp;
                existing.CliMac    = request.CliMac;
                existing.CliBraId  = request.CliBraId;
                existing.CliStatus = request.CliStatus;
                existing.CliLingua = request.CliLingua;
                existing.CliDes    = request.CliDes;
                existing.CliOff    = request.CliOff;

                _unitOfWork.Repository<Domain.Models.Client>().UpdateEntity(existing);

                if (request.AddDeviceIds != null && request.AddDeviceIds.Length > 0)
                {
                    var toAdd = request.AddDeviceIds.Select(devId => new Domain.Models.ClientDevice
                    {
                        CliId = request.CliId,
                        DevId = devId
                    }).ToList();
                    _unitOfWork.Repository<Domain.Models.ClientDevice>().AddRangeEntity(toAdd);
                }

                if (request.DelDeviceIds != null && request.DelDeviceIds.Length > 0)
                {
                    var toDelete = await _unitOfWork.Repository<Domain.Models.ClientDevice>()
                        .GetAsync(cd => cd.CliId == request.CliId && request.DelDeviceIds.Contains(cd.DevId));
                    if (toDelete.Any())
                        _unitOfWork.Repository<Domain.Models.ClientDevice>().DeleteRangeEntity(toDelete);
                }

                await _unitOfWork.TraceRepository.InsertTrace(
                    traTime: DateTime.Now,
                    traUser: request.TraUser,
                    traFunCode: "OPE",
                    traSubFun: "UPDATECLIENT",
                    traStation: request.TraStation,
                    traTabNam: "sys_CLIENT",
                    traEntCode: request.CliId,
                    traRevTrxTrace: null,
                    traDes: $"UPDATE CLIENT: ID={request.CliId} IP={request.CliIp} MAC={request.CliMac} BRA={request.CliBraId} STATUS={request.CliStatus}",
                    traExtRef: null,
                    traError: false);

                await _unitOfWork.Complete();
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Handled {CommandName}, updated client {CliId}", nameof(UpdateClientCommand), request.CliId);
                return _mapper.Map<ClientVm>(existing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {CommandName} for CliId={CliId}", nameof(UpdateClientCommand), request.CliId);
                await _unitOfWork.Rollback();
                throw new InvalidOperationException($"Errore durante l'aggiornamento della cassa: {ex.Message}");
            }
        }
    }
}
