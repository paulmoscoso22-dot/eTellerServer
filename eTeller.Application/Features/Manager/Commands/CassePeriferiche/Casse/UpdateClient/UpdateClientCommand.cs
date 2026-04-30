using eTeller.Application.Mappings.Client;
using MediatR;

namespace eTeller.Application.Features.Manager.Commands.CassePeriferiche.Casse.UpdateClient
{
    public record UpdateClientCommand(
        string CliId,
        string CliIp,
        string CliMac,
        string CliBraId,
        string CliStatus,
        string? CliLingua,
        string? CliDes,
        string? CliOff,
        int[] AddDeviceIds,
        int[] DelDeviceIds,
        string TraUser,
        string TraStation
    ) : IRequest<ClientVm>;
}
