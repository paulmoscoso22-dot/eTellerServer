using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.InsertARA
{
    public class InsertARACommandHandler : IRequestHandler<InsertARACommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InsertARACommandHandler> _logger;

        public InsertARACommandHandler(IUnitOfWork unitOfWork, ILogger<InsertARACommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(InsertARACommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Handling {CommandName} for User={TraUser}, Name={AraName}",
                nameof(InsertARACommand), request.TraUser, request.AraName);

            try
            {
                var result = await _unitOfWork.VigilanzaRepository.InsertARA(
                    request.TraUser,
                    request.TraStation,
                    request.AraRecdate,
                    request.AraName,
                    request.AraBirthdate,
                    request.AraBirthplace,
                    request.AraIddocnum,
                    request.AraNationality,
                    request.AraDocexpdate,
                    request.AraRepresents,
                    request.AraAddress,
                    request.AraRecComplete
                );

                _logger.LogInformation(
                    "Handled {CommandName}, AraId={AraId}", 
                    nameof(InsertARACommand), result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error handling {CommandName} for User={TraUser}, Name={AraName}", 
                    nameof(InsertARACommand), request.TraUser, request.AraName);
                throw;
            }
        }
    }
}
