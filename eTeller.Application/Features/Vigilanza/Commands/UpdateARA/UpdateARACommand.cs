using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.UpdateARA
{
    public record UpdateARACommand(
        string TraUser,
        string TraStation,
        int AraId,
        string AraName,
        string? AraBirthdate,
        string? AraBirthplace,
        string? AraNationality,
        string? AraIddocnum,
        string? AraDocexpdate,
        string? AraRepresents,
        string? AraAddress,
        bool AraRecComplete
    ) : IRequest<int>;
}
