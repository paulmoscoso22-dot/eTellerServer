using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Vigilanza;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetSpAntirecRulesByParameters
{
    public class GetSpAntirecRulesByParametersQueryHandler : IRequestHandler<GetSpAntirecRulesByParametersQuery, IEnumerable<SpAntirecRulesVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSpAntirecRulesByParametersQueryHandler> _logger;

        public GetSpAntirecRulesByParametersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSpAntirecRulesByParametersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SpAntirecRulesVm>> Handle(GetSpAntirecRulesByParametersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with parameters OpTypeId={OpTypeId}, CurTypeId={CurTypeId}, AcctId={AcctId}, AcctType={AcctType}",
                nameof(GetSpAntirecRulesByParametersQuery), request.arlOpTypeId, request.arlCurTypeId, request.arlAcctId, request.arlAcctType);

            var antirecRules = await _unitOfWork.VigilanzaRepository.GetAntirecRulesByParameters(
                request.arlOpTypeId,
                request.arlCurTypeId,
                request.arlAcctId,
                request.arlAcctType);

            var antirecRulesVms = _mapper.Map<IEnumerable<SpAntirecRulesVm>>(antirecRules);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetSpAntirecRulesByParametersQuery), antirecRulesVms?.Count() ?? 0);
            return antirecRulesVms ?? Enumerable.Empty<SpAntirecRulesVm>();
        }
    }
}
