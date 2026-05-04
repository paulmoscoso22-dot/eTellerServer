using eTeller.Domain.Models;

namespace eTeller.Application.Contracts
{
    public interface ICurrencyRepository
    {
        Task<Currency?> GetByKeyAsync(string curId, string curCutId);
        Task<int> UpdateCurrencyAsync(string curId, string curCutId, decimal curMinamn, string curFinezza, decimal curTolrat);
    }
}
