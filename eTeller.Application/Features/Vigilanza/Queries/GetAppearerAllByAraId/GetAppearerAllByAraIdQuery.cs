using eTeller.Application.Mappings.Vigilanza;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetAppearerAllByAraId
{
    public record GetAppearerAllByAraIdQuery(int AraId) : IRequest<AppearerAllVm?>;
}
