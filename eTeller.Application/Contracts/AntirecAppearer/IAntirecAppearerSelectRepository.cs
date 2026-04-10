using eTeller.Domain.Models.View;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.AntirecAppearer
{
    public interface IAntirecAppearerSelectRepository : IBaseSimpleRepository<AntirecAppearerView>
    {
        Task<IEnumerable<AntirecAppearerView>> GetAntirecAppearerByAreaIdAsync(int AraId);
    }
}
