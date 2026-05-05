using eTeller.Application.Mappings.CurrencyCouple;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.Divise.CoppieDivise.GetCurrencyCoupleByKey
{
    public record GetCurrencyCoupleByKeyQuery(string Cur1, string Cur2) : IRequest<CurrencyCoupleVm?>;
}
