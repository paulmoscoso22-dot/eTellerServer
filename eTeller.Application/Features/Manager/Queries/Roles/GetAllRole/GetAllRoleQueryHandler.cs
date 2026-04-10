using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Roles.GetAllRole
{
    public class GetAllRoleQueryHandler : IRequestHandler<GetAllRoleQuery, IEnumerable<SysRoleVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllRoleQueryHandler> _logger;

        public GetAllRoleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllRoleQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SysRoleVm>> Handle(GetAllRoleQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetAllRoleQuery));
            var roles = await _unitOfWork.ManagerRepository.GetAllRoleAsync(cancellationToken);

            if (roles == null) {
                _logger.LogWarning("No roles found for {QueryName}", nameof(GetAllRoleQuery));
                return Enumerable.Empty<SysRoleVm>();
            }

            var result = _mapper.Map<IEnumerable<SysRoleVm>>(roles);
            _logger.LogInformation("Handled {QueryName}, returned {Count} roles", nameof(GetAllRoleQuery), result.Count());

            return result;
        }
    }
}
