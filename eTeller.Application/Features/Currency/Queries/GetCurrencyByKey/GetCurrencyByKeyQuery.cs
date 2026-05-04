using eTeller.Application.Mappings.Currency;
using MediatR;

namespace eTeller.Application.Features.Currency.Queries.GetCurrencyByKey
{
    public record GetCurrencyByKeyQuery(string CurId, string CurCutId) : IRequest<CurrencyVm?>;
}
