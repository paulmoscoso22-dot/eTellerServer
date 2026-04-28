using MediatR;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.DeleteFunzioneSchedule
{
    public record DeleteFunzioneScheduleCommand(
        string TraUser,
        string TraStation,
        string FutId
    ) : IRequest<bool>;
}
