using eTeller.Application.Mappings.CurrencyCouple;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.Divise.CoppieDivise.GetCurrenciesDV
{
    public class GetCurrenciesDVQuery : IRequest<IEnumerable<CurrencyDvVm>> { }
}
