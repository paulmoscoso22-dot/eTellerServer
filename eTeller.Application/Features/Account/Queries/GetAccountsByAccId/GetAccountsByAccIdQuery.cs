using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.Account.Queries.GetAccountByAccId
{
    public record GetAccountsByAccIdQuery(string accId) : IRequest<IEnumerable<AccountVm>>;
    
}
