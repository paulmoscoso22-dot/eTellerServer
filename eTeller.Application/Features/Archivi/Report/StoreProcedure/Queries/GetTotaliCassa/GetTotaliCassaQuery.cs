using eTeller.Application.Mappings.TotalicCassa;
using MediatR;

namespace eTeller.Application.Features.Archivi.Report.StoreProcedure.Queries.GetTotaliCassa
{
    public record GetSpTotaliCassaQuery(
        string tocCliId,
        DateTime tocData,
        string tocCutId,
        string tocBraId
        ) : IRequest<IEnumerable<TotalicCassaVm>>;
}
