using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountsForBalance
{
    public class GetAccountsForBalanceQueryHandler : IRequestHandler<GetAccountsForBalanceQuery, IEnumerable<AccountVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAccountsForBalanceQueryHandler> _logger;

        public GetAccountsForBalanceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAccountsForBalanceQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AccountVm>> Handle(GetAccountsForBalanceQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for client {ClientId}", nameof(GetAccountsForBalanceQuery), request.clientId);
            var accounts = await _unitOfWork.AccountSpRepository.GetAccountForBalance(request.clientId);
            var accountVms = _mapper.Map<IEnumerable<AccountVm>>(accounts);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetAccountsForBalanceQuery), accountVms?.Count() ?? 0);
            return accountVms;
        }
    }
}
