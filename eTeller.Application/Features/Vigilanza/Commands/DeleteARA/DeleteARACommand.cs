using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.DeleteARA
{
    public record DeleteARACommand(
        string TraUser,
        string TraStation,
        int AraId
    ) : IRequest<bool>;
}
