using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Trace;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Trace.Queries.GetTraceWithFunction
{
    public class GetTraceWithFunctionQueryHandler : IRequestHandler<GetTraceWithFunctionQuery, IEnumerable<TraceWithFunctionVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetTraceWithFunctionQueryHandler> _logger;

        public GetTraceWithFunctionQueryHandler(IUnitOfWork unitOfWork, ILogger<GetTraceWithFunctionQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<TraceWithFunctionVm>> Handle(GetTraceWithFunctionQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetTraceWithFunctionQuery));

            //var results = await _unitOfWork.TraceSpRepository.GetTraceWithFunctionAsync(
            //    request.TraUser,
            //    request.TraFunCode,
            //    request.TraStation,
            //    request.TraTabNam,
            //    request.TraEntCode,
            //    request.TraError,
            //    request.DataFrom,
            //    request.DataTo);

            //_logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetTraceWithFunctionQuery), results?.Count() ?? 0);

            return new List<TraceWithFunctionVm>();
        }
    }
}