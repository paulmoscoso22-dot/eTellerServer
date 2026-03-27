using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Manager.Commands.UpdateSysFunction
{
    public record UpdateSysFunctionCommand(
        string TraUser,
        string TraStation,
        int FunId,
        string FunName,
        string? FunDescription,
        int FunHostcode,
        bool Offline
    ) : IRequest<bool>;
}
