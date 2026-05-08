using MediatR;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.DeleteServizio
{
    public record DeleteServizioCommand(
        string TraUser,
        string TraStation,
        string SerId
    ) : IRequest<bool>;
}
