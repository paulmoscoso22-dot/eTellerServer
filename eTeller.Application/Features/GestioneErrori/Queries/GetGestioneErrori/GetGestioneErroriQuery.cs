using eTeller.Application.Mappings.GestioneErrori;
using MediatR;

namespace eTeller.Application.Features.GestioneErrori.Queries.GetGestioneErrori;

public class GetGestioneErroriQuery : IRequest<IEnumerable<GestioneErroriVm>>
{
    public string? ErrId { get; set; }
    public string? TestoLike { get; set; }
}
