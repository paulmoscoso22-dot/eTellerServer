using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Roles.GetRoleByUsrId
{
    public class GetRoleByUsrIdQueryHandler : IRequestHandler<GetRoleByUsrIdQuery, IEnumerable<SysRoleVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetRoleByUsrIdQueryHandler> _logger;

        public GetRoleByUsrIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetRoleByUsrIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SysRoleVm>> Handle(GetRoleByUsrIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for UsrId: {UsrId}", nameof(GetRoleByUsrIdQuery), request.UsrId);

            var roles = await _unitOfWork.ManagerRepository.GetSysRoleByUsrIdAsync(request.UsrId, cancellationToken);

            var result = _mapper.Map<IEnumerable<SysRoleVm>>(roles);

            if (result == null || !result.Any())
            {
                _logger.LogWarning("No roles found for UsrId: {UsrId}", request.UsrId);
                return Enumerable.Empty<SysRoleVm>();
            }

            _logger.LogInformation("Retrieved {Count} roles for UsrId: {UsrId}", result.Count(), request.UsrId);

            return result;
        }
    }
}
