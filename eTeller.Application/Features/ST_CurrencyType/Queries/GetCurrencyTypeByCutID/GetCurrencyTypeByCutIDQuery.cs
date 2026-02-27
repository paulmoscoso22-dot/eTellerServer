using eTeller.Application.Mappings.ST_CurrencyType;
using MediatR;

namespace eTeller.Application.Features.ST_CurrencyType.Queries.GetCurrencyTypeByCutID
{
    public class GetCurrencyTypeByCutIDQuery : IRequest<ST_CurrencyTypeVm>
    {
        public string CutID { get; set; }
    }
}
