using MediatR;

namespace eTeller.Application.Features.Manager.Commands.Functions.InsertSysFunction
{
    public record InsertSysFunctionCommand(
        string TraUser,
        string TraStation,
        string FunName,
        string? FunDescription,
        int FunHostcode,
        bool Offline
    ) : IRequest<bool>;
}
