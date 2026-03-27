using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Trace;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Trace.Queries.GetTraceFunction
{
    public class GetTraceFunctionQueryHandler : IRequestHandler<GetTraceFunctionQuery, IEnumerable<ST_TRACE_FUNCTIONVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTraceFunctionQueryHandler> _logger;

        public GetTraceFunctionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTraceFunctionQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ST_TRACE_FUNCTIONVm>> Handle(GetTraceFunctionQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetTraceFunctionQuery));

            var repository = _unitOfWork.Repository<eTeller.Domain.Models.ST_TRACE_FUNCTION>();

            var results = await repository.GetAllAsync();

            var vms = _mapper.Map<IEnumerable<ST_TRACE_FUNCTIONVm>>(results);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetTraceFunctionQuery), vms?.Count() ?? 0);

            return vms;
        }
    }
}
