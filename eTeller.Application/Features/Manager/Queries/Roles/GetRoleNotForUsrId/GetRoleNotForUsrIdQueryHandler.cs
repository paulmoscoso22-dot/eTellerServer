using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Roles.GetRoleNotForUsrId
{
    public class GetRoleNotForUsrIdQueryHandler : IRequestHandler<GetRoleNotForUsrIdQuery, IEnumerable<SysRoleVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetRoleNotForUsrIdQueryHandler> _logger;

        public GetRoleNotForUsrIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetRoleNotForUsrIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SysRoleVm>> Handle(GetRoleNotForUsrIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for UsrId: {UsrId}", nameof(GetRoleNotForUsrIdQuery), request.UsrId);

            var roles = await _unitOfWork.ManagerRepository.GetSysRoleNotForUsrIdAsync(request.UsrId, cancellationToken);
            var result = _mapper.Map<IEnumerable<SysRoleVm>>(roles);

            if (result == null || !result.Any())
            {
                _logger.LogWarning("No roles found not assigned to UsrId: {UsrId}", request.UsrId);
                throw new NotFoundException(nameof(GetRoleNotForUsrIdQuery), request.UsrId);
            }

            _logger.LogInformation("Retrieved {Count} roles not assigned to UsrId: {UsrId}", result.Count(), request.UsrId);
            return result;
        }
    }
}
