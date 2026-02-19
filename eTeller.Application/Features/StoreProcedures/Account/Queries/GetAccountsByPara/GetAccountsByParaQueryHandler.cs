using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountsByPara
{
    public class GetAccountsByParaQueryHandler : IRequestHandler<GetAccountsByParaQuery, IEnumerable<AccountVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAccountsByParaQueryHandler> _logger;

        public GetAccountsByParaQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAccountsByParaQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AccountVm>> Handle(GetAccountsByParaQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with request {@Request}", nameof(GetAccountsByParaQuery), request);

            var accounts = await _unitOfWork.AccountSpRepository.GetAccountByPara(
                request.iacAccId,
                request.iacCutId,
                request.iacCurId,
                request.iacDes,
                request.iacActId,
                request.iacCliCassa,
                request.iacBraId
                );

            var accountVms = _mapper.Map<IEnumerable<AccountVm>>(accounts);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetAccountsByParaQuery), accountVms?.Count() ?? 0);
            return accountVms;
        }
    }
}
