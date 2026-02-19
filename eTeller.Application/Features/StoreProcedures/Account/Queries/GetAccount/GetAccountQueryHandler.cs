using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccount
{
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, IEnumerable<AccountVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAccountQueryHandler> _logger;

        public GetAccountQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAccountQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AccountVm>> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetAccountQuery));
            var accounts = await _unitOfWork.AccountSpRepository.GetAccountAsync();
            var accountVms = _mapper.Map<IEnumerable<AccountVm>>(accounts);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetAccountQuery), accountVms?.Count() ?? 0);
            return accountVms;
        }
    }
}
