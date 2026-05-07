using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Tabella.Commands.UpdateTabellaServVarchar
{
    public class UpdateTabellaServVarcharCommandHandler : IRequestHandler<UpdateTabellaServVarcharCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateTabellaServVarcharCommandHandler> _logger;

        public UpdateTabellaServVarcharCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateTabellaServVarcharCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateTabellaServVarcharCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} with NomeTabella: {NomeTabella}, Id: {Id}",
                nameof(UpdateTabellaServVarcharCommand), request.NomeTabella, request.Id);

            var result = await _unitOfWork.TabellaRepository.UpdateTabellaServVarchar(
                request.NomeTabella,
                request.Id,
                request.Des
            );

            _logger.LogInformation("Handled {CommandName}, success: {Result}", nameof(UpdateTabellaServVarcharCommand), result);

            return result;
        }
    }
}
