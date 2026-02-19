using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using AccountModel = eTeller.Domain.Models;
using MediatR;

namespace eTeller.Application.Features.Account.Queries.GetAccountByAccId
{
    public class GetAccountsByAccIdQueryHandler : IRequestHandler<GetAccountsByAccIdQuery, IEnumerable<AccountVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAccountsByAccIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountVm>> Handle(GetAccountsByAccIdQuery request, CancellationToken cancellationToken)
        {
            var accounts = await _unitOfWork.Repository<AccountModel.Account>().GetAsync(account => account.IacAccId == request.accId);
            return _mapper.Map<IEnumerable<AccountVm>>(accounts);
        }
    }
}
