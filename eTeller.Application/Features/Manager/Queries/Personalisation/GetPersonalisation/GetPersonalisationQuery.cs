using eTeller.Application.Mappings.Personalisation;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.Personalisation.GetPersonalisation
{
    public record GetPersonalisationQuery : IRequest<IEnumerable<PersonalisationVm>>;
}
