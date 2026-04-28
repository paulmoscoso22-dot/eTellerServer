using eTeller.Application.Mappings.Manager.Tabelle;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetFunzioniScheduleById
{
    public record GetFunzioniScheduleByIdQuery(string FutId) : IRequest<FunzioniScheduleVm?>;
}
