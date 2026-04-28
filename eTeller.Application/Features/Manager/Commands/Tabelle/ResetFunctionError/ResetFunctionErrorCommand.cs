using MediatR;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.ResetFunctionError
{
    public record ResetFunctionErrorCommand(
        string TraUser,
        string TraStation,
        string FutId
    ) : IRequest<bool>;
}
