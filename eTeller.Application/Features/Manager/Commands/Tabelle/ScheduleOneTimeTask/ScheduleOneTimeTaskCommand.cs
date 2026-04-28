using MediatR;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.ScheduleOneTimeTask
{
    public record ScheduleOneTimeTaskCommand(
        string TraUser,
        string TraStation,
        string FutId
    ) : IRequest<bool>;
}
