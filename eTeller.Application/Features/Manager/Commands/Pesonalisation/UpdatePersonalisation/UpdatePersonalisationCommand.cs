using MediatR;
using eTeller.Application.Mappings.Personalisation;

namespace eTeller.Application.Features.Manager.Commands.Pesonalisation.UpdatePersonalisation
{
    public record UpdatePersonalisationCommand(
        string ParId,
        string ParDes,
        string ParValue,
        string OriginalParId
    ) : IRequest<PersonalisationVm>;
}
