using eTeller.Application.Mappings.TotalicCassa;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.TotalicCassa.Queries.GetTotaliCassa
{
    public record GetTotaliCassaQuery(
        string tocCliId,
        DateTime tocData,
        string tocCutId,
        string tocBraId
        ) : IRequest<IEnumerable<TotalicCassaVm>>;
}
