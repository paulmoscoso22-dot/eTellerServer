using MediatR;

namespace eTeller.Application.Features.GestioneErrori.Commands.DeleteGestioneErrori;

public class DeleteGestioneErroriCommand : IRequest<bool>
{
    public string ErrId { get; set; } = string.Empty;
}
