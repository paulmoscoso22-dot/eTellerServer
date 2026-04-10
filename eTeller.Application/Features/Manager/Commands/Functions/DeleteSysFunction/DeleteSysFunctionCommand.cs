using MediatR;

namespace eTeller.Application.Features.Manager.Commands.Functions.DeleteSysFunction
{
    public record DeleteSysFunctionCommand(
        string TraUser,
        string TraStation,
        int FunId
    ) : IRequest<bool>;
}
