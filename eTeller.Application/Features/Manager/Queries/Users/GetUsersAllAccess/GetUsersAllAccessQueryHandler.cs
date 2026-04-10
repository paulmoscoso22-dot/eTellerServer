using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Users.GetUsersAllAccess
{
    public class GetUsersAllAccessQueryHandler : IRequestHandler<GetUsersAllAccessQuery, IEnumerable<USERS_AllAccessVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUsersAllAccessQueryHandler> _logger;

        public GetUsersAllAccessQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetUsersAllAccessQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<USERS_AllAccessVm>> Handle(GetUsersAllAccessQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for UsrId: {UsrId}", nameof(GetUsersAllAccessQuery), request.UsrId);
            var data = await _unitOfWork.ManagerRepository.GetUsersAllAccessAsync(request.UsrId, request.FunlikeName, request.FunlikeDes, request.Tutti, cancellationToken);
            if (data == null) {
                _logger.LogWarning("No records found for UsrId: {UsrId}", request.UsrId);
                return Enumerable.Empty<USERS_AllAccessVm>();
            }
            var result = _mapper.Map<IEnumerable<USERS_AllAccessVm>>(data);
            _logger.LogInformation("Retrieved {Count} records for UsrId: {UsrId}", result.Count(), request.UsrId);
            return result;
        }
    }
}
