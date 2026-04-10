using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Users.GetUsersActiveAndBlocked
{
    public class GetUsersActiveAndBlockedQueryHandler : IRequestHandler<GetUsersActiveAndBlockedQuery, IEnumerable<SysUsersActiveAndBlockedVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUsersActiveAndBlockedQueryHandler> _logger;

        public GetUsersActiveAndBlockedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetUsersActiveAndBlockedQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SysUsersActiveAndBlockedVm>> Handle(GetUsersActiveAndBlockedQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetUsersActiveAndBlockedQuery));
            var users = await _unitOfWork.UserRepository.GetUsersActiveAndBlockedAsync(cancellationToken);
            if (users == null) {
                _logger.LogWarning("No users found for {QueryName}", nameof(GetUsersActiveAndBlockedQuery));
                return Enumerable.Empty<SysUsersActiveAndBlockedVm>();
            }
            var result = _mapper.Map<IEnumerable<SysUsersActiveAndBlockedVm>>(users);
            _logger.LogInformation("Handled {QueryName}, returned {Count} users", nameof(GetUsersActiveAndBlockedQuery), result.Count());
            return result;
        }
    }
}