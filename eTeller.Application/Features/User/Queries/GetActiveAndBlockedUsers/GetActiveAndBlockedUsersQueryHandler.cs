using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.User.Queries.GetActiveAndBlockedUsers
{
    public class GetActiveAndBlockedUsersQueryHandler : IRequestHandler<GetActiveAndBlockedUsersQuery, IEnumerable<SysUsersActiveAndBlockedVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetActiveAndBlockedUsersQueryHandler> _logger;

        public GetActiveAndBlockedUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetActiveAndBlockedUsersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SysUsersActiveAndBlockedVm>> Handle(GetActiveAndBlockedUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetActiveAndBlockedUsersQuery));

            var repository = _unitOfWork.Repository<eTeller.Domain.Models.User>();
            var results = await repository.GetAsync(x => x.UsrStatus == "ENABLED" || x.UsrStatus == "BLOCKED");

            var vms = _mapper.Map<IEnumerable<SysUsersActiveAndBlockedVm>>(results);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetActiveAndBlockedUsersQuery), vms?.Count() ?? 0);

            return vms;
        }
    }
}
