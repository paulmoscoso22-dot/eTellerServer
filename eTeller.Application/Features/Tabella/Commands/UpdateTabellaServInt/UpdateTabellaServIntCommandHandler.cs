using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Tabella.Commands.UpdateTabellaServInt
{
    public class UpdateTabellaServIntCommandHandler : IRequestHandler<UpdateTabellaServIntCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateTabellaServIntCommandHandler> _logger;

        public UpdateTabellaServIntCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateTabellaServIntCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateTabellaServIntCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} with NomeTabella: {NomeTabella}, Id: {Id}",
                nameof(UpdateTabellaServIntCommand), request.NomeTabella, request.Id);

            var result = await _unitOfWork.TabellaRepository.UpdateTabellaServInt(
                request.NomeTabella,
                request.Id,
                request.Des
            );

            _logger.LogInformation("Handled {CommandName}, success: {Result}", nameof(UpdateTabellaServIntCommand), result);

            return result;
        }
    }
}
