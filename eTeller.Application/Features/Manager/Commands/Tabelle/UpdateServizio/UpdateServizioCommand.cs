using MediatR;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.UpdateServizio
{
    public record UpdateServizioCommand(
        string TraUser,
        string TraStation,
        string SerId,
        string SerDes,
        bool SerTrace,
        bool SerEmail,
        string? SerSyserrmail,
        string? SerApperrmail,
        bool SerEnable
    ) : IRequest<bool>;
}
