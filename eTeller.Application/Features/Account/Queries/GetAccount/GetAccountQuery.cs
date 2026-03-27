using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries
{
    public record GetAccountQuery() : IRequest<IEnumerable<AccountVm>>;

}
