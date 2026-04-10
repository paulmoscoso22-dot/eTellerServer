using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts
{
    public interface ITotalicCassaRepository : IBaseSimpleRepository<TotalicCassa>
    {
        Task<IEnumerable<TotalicCassa>> GetTotaliCassaByClientIDAndDataAndCutID(string tocCliId, DateTime tocData, string tocCutId, string tocBraId);
    }
}
