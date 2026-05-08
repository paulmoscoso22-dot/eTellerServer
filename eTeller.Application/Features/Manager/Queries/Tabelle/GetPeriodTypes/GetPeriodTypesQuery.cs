using eTeller.Application.Mappings.Manager.Tabelle;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetPeriodTypes
{
    public record GetPeriodTypesQuery() : IRequest<IEnumerable<PeriodTypeVm>>;
}
