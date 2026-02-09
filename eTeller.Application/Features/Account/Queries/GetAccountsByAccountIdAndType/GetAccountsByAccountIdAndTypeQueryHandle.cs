using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Features.Account.Queries.GetAccountByAccountIdAndType;
using eTeller.Application.Mappings;
using MediatR;
using AccountModel = eTeller.Domain.Models;

namespace eTeller.Application.Features.Account.Queries.GetAccountsByAccountIdAndType
{
    public class GetAccountsByAccountIdAndTypeQueryHandle : IRequestHandler<GetAccountsByAccountIdAndTypeQuery, IEnumerable<AccountVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAccountsByAccountIdAndTypeQueryHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountVm>> Handle(GetAccountsByAccountIdAndTypeQuery request, CancellationToken cancellationToken)
        {
            var accounts = await _unitOfWork.Repository<AccountModel.Account>().GetAsync(account => account.IacAccId == request.iacAccId && account.IacActId == request.iacActId && account.IacHostprefix == request.iacHostprefix);
            return _mapper.Map<IEnumerable<AccountVm>>(accounts);
        }
    }
}
