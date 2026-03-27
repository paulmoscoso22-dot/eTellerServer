using eTeller.Application.Mappings.Vigilanza;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetSpAntirecAppearerByParameters
{
    public record GetSpAntirecAppearerByParametersQuery(
        string? Nome1,
        string? Nome2,
        string? Nome3,
        string? Nome4,
        DateTime? AraBirthdate,
        bool? AraRecComplete,
        DateTime? MinRecdate
    ) : IRequest<IEnumerable<AppearerAllVm>>;
}
