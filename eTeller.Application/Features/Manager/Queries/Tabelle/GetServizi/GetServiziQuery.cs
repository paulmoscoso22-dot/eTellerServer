using eTeller.Application.Mappings.Manager.Tabelle;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetServizi
{
    public record GetServiziQuery() : IRequest<IEnumerable<ServiziVm>>;
}
