using eTeller.Application.Mappings.Client;
using MediatR;

namespace eTeller.Application.Features.Manager.Commands.CassePeriferiche.Casse.InsertClient
{
    public record InsertClientCommand(
        string CliId,
        string CliIp,
        string CliMac,
        string CliAuthcode,
        string CliBraId,
        string CliStatus,
        string? CliLingua,
        string? CliDes,
        string? CliOff,
        int[] DeviceIds,
        string TraUser,
        string TraStation
    ) : IRequest<ClientVm>;
}
