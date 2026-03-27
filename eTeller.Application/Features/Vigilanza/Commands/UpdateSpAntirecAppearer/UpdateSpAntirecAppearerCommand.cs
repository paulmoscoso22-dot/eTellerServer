using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.UpdateSpAntirecAppearer
{
    public record UpdateSpAntirecAppearerCommand(
        int AraId,
        DateTime AraRecdate,
        string AraName,
        DateTime? AraBirthdate,
        string? AraBirthplace,
        string? AraNationality,
        string? AraIddocnum,
        DateTime? AraDocexpdate,
        string? AraRepresents,
        string? AraAddress,
        bool AraRecComplete,
        bool AraIsupdated
    ) : IRequest<int>;
}
