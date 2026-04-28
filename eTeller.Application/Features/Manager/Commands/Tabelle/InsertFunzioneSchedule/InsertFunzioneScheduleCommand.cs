using MediatR;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.InsertFunzioneSchedule
{
    public record InsertFunzioneScheduleCommand(
        string TraUser,
        string TraStation,
        string FutId,
        string FutDes,
        string FutFunname,
        string FutScriptname,
        int FutTimeout,
        bool FutActive,
        bool FutOffline,
        bool FutTrace,
        bool FutAutatt,
        bool? FutHosval,
        string? FutPeriodtyp,
        int? FutPeriod,
        string? FutStart,
        string? FutEnd,
        string? FutNamedll,
        string? FutClassname,
        int? FutErrcount
    ) : IRequest<bool>;
}
