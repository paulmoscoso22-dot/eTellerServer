using MediatR;
using eTeller.Application.Mappings.StatoEntita;

namespace eTeller.Application.Features.StatoEntita.GetAllStatiEntita
{
    public class GetAllStatiEntitaQuery : IRequest<IEnumerable<STStatoEntitaVm>>
    {
    }
}
