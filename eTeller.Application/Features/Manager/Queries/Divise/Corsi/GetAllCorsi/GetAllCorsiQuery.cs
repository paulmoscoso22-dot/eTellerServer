using eTeller.Application.Mappings.Manager;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.Divise.Corsi.GetAllCorsi
{
    public record GetAllCorsiQuery(
        string? CurId,
        string? CurLondes,
        string? CurCutId,
        DateTime? DateFrom,
        DateTime? DateTo) : IRequest<IEnumerable<CorsiVm>>;
}
