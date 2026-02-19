using eTeller.Application.Mappings.Currency;
using MediatR;

namespace eTeller.Application.Features.Currency.Queries.GetAllCurrencies
{
    public class GetAllCurrenciesQuery : IRequest<IEnumerable<CurrencyVm>>
    {
    }
}
