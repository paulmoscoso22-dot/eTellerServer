using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Tabella.Commands.InsertTabellaServInt
{
    public class InsertTabellaServIntCommandHandler : IRequestHandler<InsertTabellaServIntCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InsertTabellaServIntCommandHandler> _logger;

        public InsertTabellaServIntCommandHandler(IUnitOfWork unitOfWork, ILogger<InsertTabellaServIntCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(InsertTabellaServIntCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} with NomeTabella: {NomeTabella}, Id: {Id}",
                nameof(InsertTabellaServIntCommand), request.NomeTabella, request.Id);

            await _unitOfWork.TabellaRepository.InsertTabellaServInt(
                request.NomeTabella,
                request.Id,
                request.Des
            );

            _logger.LogInformation("Handled {CommandName} successfully", nameof(InsertTabellaServIntCommand));

            return true;
        }
    }
}
