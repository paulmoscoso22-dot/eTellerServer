using MediatR;

namespace eTeller.Application.Features.Manager.Commands.CassePeriferiche.Casse.DeleteClient
{
    public record DeleteClientCommand(
        string CliId,
        string TraUser,
        string TraStation
    ) : IRequest<bool>;
}
