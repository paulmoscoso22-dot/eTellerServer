using eTeller.Application.Mappings.CurrencyCouple;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.Divise.CoppieDivise.GetAllCurrencyCouples
{
    public class GetAllCurrencyCouplesQuery : IRequest<IEnumerable<CurrencyCoupleVm>> { }
}
