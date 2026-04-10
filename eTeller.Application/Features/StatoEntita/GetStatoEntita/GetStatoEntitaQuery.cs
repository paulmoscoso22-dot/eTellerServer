using MediatR;
using eTeller.Application.Mappings.StatoEntita;

namespace eTeller.Application.Features.StatoEntita.GetStatoEntita
{
    public class GetStatoEntitaQuery : IRequest<IEnumerable<STStatoEntitaVm>>
    {
        public string SteId { get; set; }

        public GetStatoEntitaQuery(string steId)
        {
            SteId = steId;
        }
    }
}
