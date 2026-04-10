using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Roles.GetUserByRole
{
    public class GetUserByRoleQueryHandler : IRequestHandler<GetUserByRoleQuery, IEnumerable<UserSelectRoleVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByRoleQueryHandler> _logger;

        public GetUserByRoleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetUserByRoleQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<UserSelectRoleVm>> Handle(GetUserByRoleQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with RoleId: {RoleId}", nameof(GetUserByRoleQuery), request.RoleId);

            var users = await _unitOfWork.ManagerRepository.GetUsersByRoleIdAsync(request.RoleId, cancellationToken);
            if (users == null || !users.Any())
            {
                _logger.LogWarning("No users found for RoleId: {RoleId}", request.RoleId);
                return Enumerable.Empty<UserSelectRoleVm>();
            }
            var result = _mapper.Map<IEnumerable<UserSelectRoleVm>>(users);
            _logger.LogInformation("Handled {QueryName}, returned {Count} users", nameof(GetUserByRoleQuery), result.Count());
            return result;
        }
    }
}
