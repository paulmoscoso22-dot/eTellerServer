using eTeller.Domain.Models;

namespace eTeller.Application.Contracts.Tabella
{
    public interface ITabellaRepository
    {
        Task<IEnumerable<Na_TabellaServVarchar>> GetTabellaServVarchar(string nomeTabella, string? id, string? desLike);
        Task<Na_TabellaServVarchar?> GetTabellaServVarcharById(string id, string nomeTabella);
    }
}
