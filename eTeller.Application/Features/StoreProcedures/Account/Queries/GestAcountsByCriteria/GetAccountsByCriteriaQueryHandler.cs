using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GestAcountsByCriteria
{
    public class GetAccountsByCriteriaQueryHandler : IRequestHandler<GetAccountsByCriteriaQuery, IEnumerable<AccountVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAccountsByCriteriaQueryHandler> _logger;

        public GetAccountsByCriteriaQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAccountsByCriteriaQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AccountVm>> Handle(GetAccountsByCriteriaQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with parameters AccType={AccType}, Branch={Branch}, CliId={CliId}, Currency={Currency}, CurrencyType={CurrencyType}",
                nameof(GetAccountsByCriteriaQuery), request.accType, request.branch, request.cliId, request.currency, request.currencyType);

            var accounts = await _unitOfWork.AccountSpRepository.GetAccountByCriteria(
                request.accType,
                request.branch,
                request.cliId,
                request.currency,
                request.currencyType);
            var accountVms = _mapper.Map<IEnumerable<AccountVm>>(accounts);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetAccountsByCriteriaQuery), accountVms?.Count() ?? 0);
            return accountVms;
        }
    }
}
