using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GestAcountsByCriteria
{
    public class GetAccountsByCriteriaQueryHandle : IRequestHandler<GetAccountsByCriteriaQuery, IEnumerable<AccountVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAccountsByCriteriaQueryHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountVm>> Handle(GetAccountsByCriteriaQuery request, CancellationToken cancellationToken)
        {
            var accounts = await _unitOfWork.AccountSpRepository.GetAccountByCriteria(
                request.accType,
                request.branch,
                request.cliId,
                request.currency,
                request.currencyType);
            var accountVms = _mapper.Map<IEnumerable<AccountVm>>(accounts);
            return accountVms;
        }
    }
}
