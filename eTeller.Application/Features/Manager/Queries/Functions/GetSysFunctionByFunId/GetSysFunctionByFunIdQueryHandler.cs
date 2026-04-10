using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Functions.GetSysFunctionByFunId
{
    public class GetSysFunctionByFunIdQueryHandler : IRequestHandler<GetSysFunctionByFunIdQuery, IEnumerable<SysFunctionsVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSysFunctionByFunIdQueryHandler> _logger;

        public GetSysFunctionByFunIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSysFunctionByFunIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SysFunctionsVm>> Handle(GetSysFunctionByFunIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for FunId={FunId}", nameof(GetSysFunctionByFunIdQuery), request.FunId);
            var items = await _unitOfWork.Repository<SysFunctions>().GetAsync(f => f.FunId == request.FunId);
            if (items == null) {
                _logger.LogWarning("No sys functions found for FunId={FunId}", request.FunId);
                return Enumerable.Empty<SysFunctionsVm>();
            }
            var result = _mapper.Map<IEnumerable<SysFunctionsVm>>(items);
            _logger.LogInformation("Retrieved {Count} sys functions for FunId={FunId}", result.Count(), request.FunId);
            return result;
        }
    }
}
