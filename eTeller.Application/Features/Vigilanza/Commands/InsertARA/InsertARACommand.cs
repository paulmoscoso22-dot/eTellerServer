using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.InsertARA
{
    public record InsertARACommand(
        string TraUser,
        string TraStation,
        DateTime AraRecdate,
        string AraName,
        string? AraBirthdate,
        string? AraBirthplace,
        string? AraIddocnum,
        string? AraNationality,
        string? AraDocexpdate,
        string? AraRepresents,
        string? AraAddress,
        bool AraRecComplete
    ) : IRequest<int>;
}
