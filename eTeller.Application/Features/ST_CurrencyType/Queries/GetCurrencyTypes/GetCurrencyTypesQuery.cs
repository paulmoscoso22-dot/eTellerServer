using eTeller.Application.Mappings.ST_CurrencyType;
using MediatR;

namespace eTeller.Application.Features.ST_CurrencyType.Queries.GetCurrencyTypes
{
    public class GetCurrencyTypesQuery : IRequest<IEnumerable<ST_CurrencyTypeVm>>
    {
    }
}
