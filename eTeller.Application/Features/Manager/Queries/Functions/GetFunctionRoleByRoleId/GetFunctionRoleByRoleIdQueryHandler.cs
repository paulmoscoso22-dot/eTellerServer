using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Functions.GetFunctionRoleByRoleId
{
    public class GetFunctionRoleByRoleIdQueryHandler : IRequestHandler<GetFunctionRoleByRoleIdQuery, IEnumerable<FunctionRoleVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetFunctionRoleByRoleIdQueryHandler> _logger;

        public GetFunctionRoleByRoleIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetFunctionRoleByRoleIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<FunctionRoleVm>> Handle(GetFunctionRoleByRoleIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for RoleId={RoleId}", nameof(GetFunctionRoleByRoleIdQuery), request.RoleId);
            var functions = await _unitOfWork.ManagerRepository.GetFunctionRoleByRoleIdAsync(
                request.RoleId,
                request.FunLikeName,
                request.FunLikeDes,
                cancellationToken);
            if (functions == null || !functions.Any())
            {
                _logger.LogWarning("No functions found for RoleId={RoleId} with criteria FunLikeName={FunLikeName}, FunLikeDes={FunLikeDes}", request.RoleId, request.FunLikeName, request.FunLikeDes);
                throw new NotFoundException($"No functions found for RoleId={request.RoleId} with criteria FunLikeName={request.FunLikeName}, FunLikeDes={request.FunLikeDes}", null);
            }
             var result = _mapper.Map<IEnumerable<FunctionRoleVm>>(functions);
            _logger.LogInformation("Handled {QueryName}, returned {Count} functions", nameof(GetFunctionRoleByRoleIdQuery), result.Count());
            return result;
        }
    }
}
