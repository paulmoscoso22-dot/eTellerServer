using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Tabella.Commands.UpdateOperationType
{
    public class UpdateOperationTypeCommandHandler : IRequestHandler<UpdateOperationTypeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateOperationTypeCommandHandler> _logger;

        public UpdateOperationTypeCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateOperationTypeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateOperationTypeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} with OptId: {OptId}", nameof(UpdateOperationTypeCommand), request.OptId);

            var existing = await _unitOfWork.Repository<Domain.Models.ST_OperationType>()
                .GetAsync(x => x.OptId == request.OptId);
            var entity = existing.FirstOrDefault()
                ?? throw new InvalidOperationException($"Il tipo operazione con ID '{request.OptId}' non esiste.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                entity.OptDes      = request.OptDes;
                entity.OptHoscod   = request.OptHoscod;
                entity.OptIscredit = request.OptIscredit;
                entity.OptAptId    = request.OptAptId;
                entity.OptPrtdv    = request.OptPrtdv;
                entity.OptAdvId    = request.OptAdvId;

                _unitOfWork.Repository<Domain.Models.ST_OperationType>().UpdateEntity(entity);

                var saved = await _unitOfWork.Complete();
                if (saved == 0)
                {
                    _logger.LogError("Failed to update operation type {OptId}", request.OptId);
                    throw new InvalidOperationException($"Impossibile aggiornare il tipo operazione '{request.OptId}'.");
                }

                await _unitOfWork.TraceRepository.InsertTrace(
                    traTime: DateTime.Now,
                    traUser: request.TraUser,
                    traFunCode: "MGR",
                    traSubFun: "UPDATEOPERATIONTYPE",
                    traStation: request.TraStation,
                    traTabNam: "ST_OPERATIONTYPE",
                    traEntCode: request.OptId,
                    traRevTrxTrace: null,
                    traDes: $"UPDATE OPERATIONTYPE: ID={request.OptId} DES={request.OptDes} HOSCOD={request.OptHoscod} ISCREDIT={request.OptIscredit} APTID={request.OptAptId}",
                    traExtRef: null,
                    traError: false);

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Handled {CommandName}, updated operation type {OptId}", nameof(UpdateOperationTypeCommand), request.OptId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {CommandName} for OptId={OptId}", nameof(UpdateOperationTypeCommand), request.OptId);
                await _unitOfWork.Rollback();
                throw new InvalidOperationException($"Errore durante l'aggiornamento del tipo operazione: {ex.Message}");
            }
        }
    }
}
