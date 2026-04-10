using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Roles.GetSysRoleByFunId
{
    public class GetSysRoleByFunIdQueryHandler : IRequestHandler<GetSysRoleByFunIdQuery, IEnumerable<SysRoleVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSysRoleByFunIdQueryHandler> _logger;

        public GetSysRoleByFunIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSysRoleByFunIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SysRoleVm>> Handle(GetSysRoleByFunIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for FunId={FunId}", nameof(GetSysRoleByFunIdQuery), request.FunId);
            var roles = await _unitOfWork.ManagerRepository.GetSysRoleByFunIdAsync(request.FunId, cancellationToken);
            if (roles == null || !roles.Any())
            {
                _logger.LogWarning("No roles found for FunId={FunId}", request.FunId);
                return Enumerable.Empty<SysRoleVm>();
            }
            var result = _mapper.Map<IEnumerable<SysRoleVm>>(roles);
            _logger.LogInformation("Retrieved {Count} roles for FunId={FunId}", result.Count(), request.FunId);
            return result;
        }
    }
}
