using eTeller.Domain.Models;

namespace eTeller.Application.Contracts.StoreProcedures
{
    public interface ICurrencySpRepository
    {
        Task<List<Currency>> GetAllCurrencies();
    }
}
