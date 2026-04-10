using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Users.GetUserByHostId
{
    public class GetUserByHostIdQueryHandler : IRequestHandler<GetUserByHostIdQuery, IEnumerable<SysUserByIdVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByHostIdQueryHandler> _logger;

        public GetUserByHostIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetUserByHostIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SysUserByIdVm>> Handle(GetUserByHostIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for UsrHostId: {UsrHostId}", nameof(GetUserByHostIdQuery), request.UsrHostId);
            var repo = _unitOfWork.Repository<eTeller.Domain.Models.User>();
            var users = await repo.GetAsync(u => u.UsrHostId == request.UsrHostId);
            if (users == null || !users.Any())
            {
                _logger.LogWarning("No users found for UsrHostId: {UsrHostId}", request.UsrHostId);
                throw new NotFoundException(nameof(GetUserByHostIdQuery), request.UsrHostId);
            }
            var result = _mapper.Map<IEnumerable<SysUserByIdVm>>(users);
            _logger.LogInformation("Retrieved {Count} records for UsrHostId: {UsrHostId}", result.Count(), request.UsrHostId);
            return result;
        }
    }
}
