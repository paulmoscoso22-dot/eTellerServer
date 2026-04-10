using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Users.GetAllUsersByUsrId
{
    public class GetAllUsersByUsrIdQueryHandler : IRequestHandler<GetAllUsersByUsrIdQuery, IEnumerable<InfoAutorizzazioneUtenteVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllUsersByUsrIdQueryHandler> _logger;

        public GetAllUsersByUsrIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllUsersByUsrIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<InfoAutorizzazioneUtenteVm>> Handle(GetAllUsersByUsrIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for UsrId: {UsrId}", nameof(GetAllUsersByUsrIdQuery), request.UsrId);
            var userAuthorizations = await _unitOfWork.ManagerRepository.GetAllUsersByUsrIdAsync(
                request.UsrId,
                request.FunlikeName,
                request.FunlikeDes,
                request.Tutti,
                cancellationToken);
            if (userAuthorizations == null) {
                _logger.LogWarning("No authorizations found for UsrId: {UsrId}", request.UsrId);
                return Enumerable.Empty<InfoAutorizzazioneUtenteVm>();
            }
            var result = _mapper.Map<IEnumerable<InfoAutorizzazioneUtenteVm>>(userAuthorizations);
            _logger.LogInformation("Retrieved {Count} authorizations for UsrId: {UsrId}", result.Count(), request.UsrId);
            return result;
        }
    }
}
