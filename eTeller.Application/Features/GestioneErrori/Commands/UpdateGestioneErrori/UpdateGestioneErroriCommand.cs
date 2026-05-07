using MediatR;

namespace eTeller.Application.Features.GestioneErrori.Commands.UpdateGestioneErrori;

public class UpdateGestioneErroriCommand : IRequest<bool>
{
    public string ErrId { get; set; } = string.Empty;
    public string? ErrTyp { get; set; }
    public bool ErrCanFlag { get; set; }
    public bool ErrConFlag { get; set; }
    public bool ErrForFlag { get; set; }
    public string? ErrFocId { get; set; }
    public string? ErrDesSol { get; set; }
    public string ErrDescIt { get; set; } = string.Empty;
    public string? ErrDescEn { get; set; }
    public string? ErrDescFr { get; set; }
    public string? ErrDescDe { get; set; }
}
