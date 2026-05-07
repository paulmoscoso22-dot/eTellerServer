using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Tabella.Commands.InsertOperationType
{
    public class InsertOperationTypeCommandHandler : IRequestHandler<InsertOperationTypeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InsertOperationTypeCommandHandler> _logger;

        public InsertOperationTypeCommandHandler(IUnitOfWork unitOfWork, ILogger<InsertOperationTypeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(InsertOperationTypeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} with OptId: {OptId}", nameof(InsertOperationTypeCommand), request.OptId);

            var existing = await _unitOfWork.Repository<Domain.Models.ST_OperationType>()
                .GetAsync(x => x.OptId == request.OptId);
            if (existing.Any())
                throw new InvalidOperationException($"Il tipo operazione con ID '{request.OptId}' esiste già.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var entity = new Domain.Models.ST_OperationType
                {
                    OptId       = request.OptId,
                    OptDes      = request.OptDes,
                    OptHoscod   = request.OptHoscod,
                    OptIscredit = request.OptIscredit,
                    OptAptId    = request.OptAptId,
                    OptPrtdv    = request.OptPrtdv,
                    OptAdvId    = request.OptAdvId
                };

                _unitOfWork.Repository<Domain.Models.ST_OperationType>().AddEntity(entity);

                var saved = await _unitOfWork.Complete();
                if (saved == 0)
                {
                    _logger.LogError("Failed to insert operation type {OptId}", request.OptId);
                    throw new InvalidOperationException($"Impossibile salvare il tipo operazione '{request.OptId}'.");
                }

                await _unitOfWork.TraceRepository.InsertTrace(
                    traTime: DateTime.Now,
                    traUser: request.TraUser,
                    traFunCode: "MGR",
                    traSubFun: "INSERTOPERATIONTYPE",
                    traStation: request.TraStation,
                    traTabNam: "ST_OPERATIONTYPE",
                    traEntCode: request.OptId,
                    traRevTrxTrace: null,
                    traDes: $"INSERT OPERATIONTYPE: ID={request.OptId} DES={request.OptDes} HOSCOD={request.OptHoscod} ISCREDIT={request.OptIscredit} APTID={request.OptAptId}",
                    traExtRef: null,
                    traError: false);

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Handled {CommandName}, inserted operation type {OptId}", nameof(InsertOperationTypeCommand), request.OptId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {CommandName} for OptId={OptId}", nameof(InsertOperationTypeCommand), request.OptId);
                await _unitOfWork.Rollback();
                throw new InvalidOperationException($"Errore durante l'inserimento del tipo operazione: {ex.Message}");
            }
        }
    }
}
