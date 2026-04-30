using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.CassePeriferiche.Casse.DeleteClient
{
    public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteClientCommandHandler> _logger;

        public DeleteClientCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteClientCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for CliId={CliId}", nameof(DeleteClientCommand), request.CliId);

            var existing = (await _unitOfWork.Repository<Domain.Models.Client>()
                .GetAsync(c => c.CliId == request.CliId)).FirstOrDefault();

            if (existing == null)
            {
                _logger.LogWarning("Client {CliId} not found for deletion", request.CliId);
                throw new InvalidOperationException($"La cassa '{request.CliId}' non trovata.");
            }

            _unitOfWork.Repository<Domain.Models.Client>().DeleteEntity(existing);
            var saved = await _unitOfWork.Complete();

            if (saved == 0)
            {
                _logger.LogError("Failed to delete client {CliId}", request.CliId);
                throw new InvalidOperationException($"Impossibile eliminare la cassa '{request.CliId}'.");
            }

            await _unitOfWork.TraceRepository.InsertTrace(
                traTime: DateTime.Now,
                traUser: request.TraUser,
                traFunCode: "OPE",
                traSubFun: "DELETECLIENT",
                traStation: request.TraStation,
                traTabNam: "sys_CLIENT",
                traEntCode: request.CliId,
                traRevTrxTrace: null,
                traDes: $"DELETE CLIENT: ID={request.CliId}",
                traExtRef: null,
                traError: false);

            _logger.LogInformation("Handled {CommandName}, deleted client {CliId}", nameof(DeleteClientCommand), request.CliId);
            return true;
        }
    }
}
