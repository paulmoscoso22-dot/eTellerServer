using eTeller.Application.Mappings.Currency;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Currency.Queries.GetCurrencies
{
    public class GetCurrenciesQuery : IRequest<IEnumerable<CurrencyVm>>
    {
    }
}
