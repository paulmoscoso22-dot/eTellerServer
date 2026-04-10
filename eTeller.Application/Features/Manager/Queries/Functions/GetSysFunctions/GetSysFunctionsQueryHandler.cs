using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Functions.GetSysFunctions
{
    public class GetSysFunctionsQueryHandler : IRequestHandler<GetSysFunctionsQuery, IEnumerable<SysFunctionsVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSysFunctionsQueryHandler> _logger;

        public GetSysFunctionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSysFunctionsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SysFunctionsVm>> Handle(GetSysFunctionsQuery request, CancellationToken cancellationToken)
        {
            
            _logger.LogInformation("Handling {QueryName}", nameof(GetSysFunctionsQuery));
            var sysFunctions = await _unitOfWork.ManagerRepository.GetSysFunctionsAsync(cancellationToken);
            var result = _mapper.Map<IEnumerable<SysFunctionsVm>>(sysFunctions);
            if (result == null)
            {
                _logger.LogWarning("No sys functions found");
                throw new NotFoundException(nameof(GetSysFunctionsQuery), "none");
            }
            _logger.LogInformation("Retrieved {Count} sys functions", result.Count());
            return result;
        }
    }
}
