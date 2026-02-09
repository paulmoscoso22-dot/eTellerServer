using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccount
{
    public class GetAccountQueryHandle : IRequestHandler<GetAccountQuery, IEnumerable<AccountVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAccountQueryHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountVm>> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var accounts = await _unitOfWork.AccountSpRepository.GetAccountAsync();
            var accountVms = _mapper.Map<IEnumerable<AccountVm>>(accounts);
            return accountVms;
        }
    }
}
