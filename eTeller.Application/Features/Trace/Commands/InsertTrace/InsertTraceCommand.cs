using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Trace.Commands.InsertTrace
{
    public record InsertTraceCommand(
        DateTime TraTime,
        string TraUser,
        string TraFunCode,
        string? TraSubFun,
        string TraStation,
        string TraTabNam,
        string TraEntCode,
        string? TraRevTrxTrace,
        string? TraDes,
        string? TraExtRef,
        bool TraError
    ) : IRequest<int>;
}
