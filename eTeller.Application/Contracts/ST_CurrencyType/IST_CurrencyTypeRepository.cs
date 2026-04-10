using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.ST_CurrencyType
{
    public interface IST_CurrencyTypeRepository : IBaseSimpleRepository<eTeller.Domain.Models.ST_CurrencyType>
    {
        Task<eTeller.Domain.Models.ST_CurrencyType> GetByCutID(string CutID);
    }
}
