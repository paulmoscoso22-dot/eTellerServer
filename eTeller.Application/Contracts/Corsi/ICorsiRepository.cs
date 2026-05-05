using eTeller.Domain.Models.StoredProcedure;

namespace eTeller.Application.Contracts.Corsi
{
    public interface ICorsiRepository
    {
        Task<IEnumerable<CorsiResult>> GetAllAsync(string? curId, string? curLondes, string? curCutId, DateTime? dateFrom, DateTime? dateTo);
    }
}
