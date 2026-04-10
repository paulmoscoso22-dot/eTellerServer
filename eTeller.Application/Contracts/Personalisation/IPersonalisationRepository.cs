using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.Personalisation
{
    public interface IPersonalisationRepository : IBaseSimpleRepository<eTeller.Domain.Models.Personalisation>
    {
        Task<eTeller.Domain.Models.Personalisation?> PersonalisationUpdateAsync(string parId, string parDes, string parValue, string originalParId);
    }
}
