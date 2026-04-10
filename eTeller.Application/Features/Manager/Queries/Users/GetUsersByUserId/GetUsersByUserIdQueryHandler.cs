using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;
using eTeller.Application.Exceptions;

namespace eTeller.Application.Features.Manager.Queries.Users.GetUsersByUserId
{
    public class GetUsersByUserIdQueryHandler : IRequestHandler<GetUsersByUserIdQuery, IEnumerable<SysUserByIdVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUsersByUserIdQueryHandler> _logger;

        public GetUsersByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetUsersByUserIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SysUserByIdVm>> Handle(GetUsersByUserIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for UsrId: {UsrId}", nameof(GetUsersByUserIdQuery), request.UsrId);
            var repository = _unitOfWork.Repository<Domain.Models.User>();
            var users = await repository.GetAsync(u => u.UsrId == request.UsrId);
            if (users == null || !users.Any())
            {
                _logger.LogWarning("No users found for UsrId: {UsrId}", request.UsrId);
                throw new NotFoundException(nameof(GetUsersByUserIdQuery), request.UsrId);
            }
            var result = _mapper.Map<IEnumerable<SysUserByIdVm>>(users);
            _logger.LogInformation("Retrieved {Count} records for UsrId: {UsrId}", result.Count(), request.UsrId);
            return result;
        }
    }
}
