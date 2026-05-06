using eTeller.Application.Mappings.GestioneErrori;
using MediatR;

namespace eTeller.Application.Features.GestioneErrori.Queries.GetGestioneErroriById;

public class GetGestioneErroriByIdQuery : IRequest<GestioneErroriVm?>
{
    public string ErrId { get; set; } = string.Empty;
}
