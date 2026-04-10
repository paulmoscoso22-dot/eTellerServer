using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Roles.GetRoleByName
{
    public class GetRoleByNameQueryHandler : IRequestHandler<GetRoleByNameQuery, SysRoleVm?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetRoleByNameQueryHandler> _logger;

        public GetRoleByNameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetRoleByNameQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SysRoleVm?> Handle(GetRoleByNameQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with RoleName={RoleName}", nameof(GetRoleByNameQuery), request.RoleName);

            var roles = await _unitOfWork.Repository<Domain.Models.sys_ROLE>()
                .GetAsync(r => r.RoleName == request.RoleName);

            if (roles == null) {    
                _logger.LogInformation("Handled {QueryName}, no roles found with RoleName={RoleName}", nameof(GetRoleByNameQuery), request.RoleName);
                return null;
            }
            var role = roles.FirstOrDefault();
            var result = _mapper.Map<SysRoleVm?>(role);
            _logger.LogInformation("Handled {QueryName}, found={Found}", nameof(GetRoleByNameQuery), result != null);

            return result;
        }
    }
}