using eTeller.Application.Mappings.BookingRc;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetAccountTypes
{
    public record GetAccountTypesQuery() : IRequest<IEnumerable<AccountTypeVm>>;
}
