using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Manager.Queries.GetUsersRoleFunId
{
    public class GetUsersRoleFunIdQueryHandler : IRequestHandler<GetUsersRoleFunIdQuery, IEnumerable<UsersRoleFunctionVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUsersRoleFunIdQueryHandler> _logger;

        public GetUsersRoleFunIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetUsersRoleFunIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<UsersRoleFunctionVm>> Handle(GetUsersRoleFunIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for FunId={FunId}", nameof(GetUsersRoleFunIdQuery), request.FunId);

            var items = await _unitOfWork.ManagerSpRepository.GetUsersRoleByFunIdAsync(request.FunId, cancellationToken);

            var result = _mapper.Map<IEnumerable<UsersRoleFunctionVm>>(items);

            _logger.LogInformation("Retrieved {Count} items for FunId={FunId}", result.Count(), request.FunId);

            return result;
        }
    }
}
