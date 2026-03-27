using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.InsertHisAntirecAppearer
{
    public record InsertHisAntirecAppearerCommand(
        DateTime HisDate,
        int AraId,
        DateTime AraRecdate,
        string AraName,
        DateTime? AraBirthdate,
        string? AraBirthplace,
        string? AraNationality,
        string? AraIddocnum,
        DateTime? AraDocexpdate,
        bool AraRecComplete,
        string? AraRepresents,
        string? AraAddress
    ) : IRequest<int>;
}
