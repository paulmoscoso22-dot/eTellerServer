using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountsForBalance
{
    public record GetAccountsForBalanceQuery(string clientId) : IRequest<IEnumerable<AccountVm>>;
}
