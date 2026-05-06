using eTeller.Domain.Models.StoredProcedure;

namespace eTeller.Application.Contracts.ForceTrx
{
    public interface IForceTrxRepository
    {
        Task<IEnumerable<ForceTrxResult>> GetAllAsync(string lanCode);
        Task<IEnumerable<ForceTrxResult>> GetByIdAsync(string lanCode, int trfId);
    }
}
