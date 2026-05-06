using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Tabella.Commands.InsertTabellaServVarchar
{
    public class InsertTabellaServVarcharCommandHandler : IRequestHandler<InsertTabellaServVarcharCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InsertTabellaServVarcharCommandHandler> _logger;

        public InsertTabellaServVarcharCommandHandler(IUnitOfWork unitOfWork, ILogger<InsertTabellaServVarcharCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(InsertTabellaServVarcharCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} with NomeTabella: {NomeTabella}, Id: {Id}",
                nameof(InsertTabellaServVarcharCommand), request.NomeTabella, request.Id);

            await _unitOfWork.TabellaRepository.InsertTabellaServVarchar(
                request.NomeTabella,
                request.Id,
                request.Des
            );

            _logger.LogInformation("Handled {CommandName} successfully", nameof(InsertTabellaServVarcharCommand));

            return true;
        }
    }
}
