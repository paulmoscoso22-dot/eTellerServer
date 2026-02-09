using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.Account.Queries.GetAccountByAccountIdAndType
{
    public record GetAccountsByAccountIdAndTypeQuery(string iacAccId, string iacActId, string iacHostprefix) : IRequest<IEnumerable<AccountVm>>;
}
