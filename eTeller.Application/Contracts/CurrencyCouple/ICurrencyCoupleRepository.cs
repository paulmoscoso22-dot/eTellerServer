using eTeller.Application.Mappings.CurrencyCouple;

namespace eTeller.Application.Contracts.CurrencyCouple
{
    public interface ICurrencyCoupleRepository
    {
        Task<IEnumerable<CurrencyCoupleVm>> GetAllAsync();
        Task<CurrencyCoupleVm?> GetByKeyAsync(string cur1, string cur2);
        Task<IEnumerable<CurrencyDvVm>> GetCurrenciesDVAsync();
        Task<CurrencyCoupleVm> InsertAsync(string cur1, string cur2, string? londes, string? shodes, decimal? size, string? excdir);
        Task<CurrencyCoupleVm> UpdateAsync(string cur1, string cur2, string? londes, string? shodes, decimal? size, string? excdir);
        Task<bool> DeleteAsync(string cur1, string cur2);
    }
}
