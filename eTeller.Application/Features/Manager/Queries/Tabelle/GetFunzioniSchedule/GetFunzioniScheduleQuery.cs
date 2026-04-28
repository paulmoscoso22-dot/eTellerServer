using eTeller.Application.Mappings.Manager.Tabelle;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetFunzioniSchedule
{
    public record GetFunzioniScheduleQuery(
        string? NomeLike,
        string? DesLike
    ) : IRequest<IEnumerable<FunzioniScheduleVm>>;
}
