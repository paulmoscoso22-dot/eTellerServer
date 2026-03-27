using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Manager.Commands.DeleteSysFunction
{
    public record DeleteSysFunctionCommand(
        string TraUser,
        string TraStation,
        int FunId
    ) : IRequest<bool>;
}
