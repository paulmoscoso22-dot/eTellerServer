using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Trace;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Trace.Queries.GetTraceById
{
    public class GetTraceByIdQueryHandler : IRequestHandler<GetTraceByIdQuery, TraceVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTraceByIdQueryHandler> _logger;

        public GetTraceByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTraceByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TraceVm> Handle(GetTraceByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with TraId={TraId}",
                nameof(GetTraceByIdQuery), request.TraId);

            var repository = _unitOfWork.Repository<Domain.Models.Trace>();
            var result = await repository.GetAsync(t => t.TraId == request.TraId);

            var trace = result.FirstOrDefault();
            var vm = _mapper.Map<TraceVm>(trace);
            _logger.LogInformation("Handled {QueryName}, found: {Found}", nameof(GetTraceByIdQuery), trace != null);

            return vm;
        }
    }
}