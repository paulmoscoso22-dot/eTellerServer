using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountsByIacId
{
    public class GetAccountsByIacIdQueryHandler : IRequestHandler<GetAccountsByIacIdQuery, IEnumerable<AccountVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAccountsByIacIdQueryHandler> _logger;

        public GetAccountsByIacIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAccountsByIacIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AccountVm>> Handle(GetAccountsByIacIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with IacId={IacId}", nameof(GetAccountsByIacIdQuery), request.iacId);
            var accounts = await _unitOfWork.AccountSpRepository.GetAccountByIacId(request.iacId);
            var accountVms = _mapper.Map<IEnumerable<AccountVm>>(accounts);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetAccountsByIacIdQuery), accountVms?.Count() ?? 0);
            return accountVms;
        }
    }
}
