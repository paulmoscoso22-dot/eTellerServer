using eTeller.Domain.Models;

namespace eTeller.Application.Contracts.BookingRc
{
    public interface IBookingRcRepository
    {
        Task<IEnumerable<StAccountType>> GetAccountTypesAsync();
        Task<int> InsertAsync(StBookingRc item);
        Task<int> UpdateAsync(StBookingRc item);
    }
}
