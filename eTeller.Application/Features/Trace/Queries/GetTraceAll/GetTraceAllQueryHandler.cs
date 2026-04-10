using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Trace;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Trace.Queries.GetTraceAll
{
    public class GetTraceAllQueryHandler : IRequestHandler<GetTraceAllQuery, IEnumerable<TraceVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTraceAllQueryHandler> _logger;

        public GetTraceAllQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTraceAllQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TraceVm>> Handle(GetTraceAllQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with filters: User={TraUser}, FunCode={TraFunCode}, TabNam={TraTabNam}",
                nameof(GetTraceAllQuery), request.TraUser, request.TraFunCode, request.TraTabNam);

            var results = await _unitOfWork.TraceRepository.GetTraceAll(
                request.TraUser,
                request.TraFunCode,
                request.TraStation,
                request.TraTabNam,
                request.TraEntCode,
                request.TraError,
                request.DataFrom,
                request.DataTo
            );

            var vms = _mapper.Map<IEnumerable<TraceVm>>(results);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetTraceAllQuery), vms?.Count() ?? 0);

            return vms;
        }
    }
}
