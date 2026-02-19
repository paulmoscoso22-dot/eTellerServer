using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountsByIacId
{
    public record GetAccountsByIacIdQuery(int iacId) : IRequest<IEnumerable<AccountVm>>;
}
