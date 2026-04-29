using MediatR;

namespace eTeller.Application.Features.Manager.Commands.CassePeriferiche.Casse.UpdateClient;

public record UpdateClientCommand(
    string CliId,
    string CliIp,
    string CliMac,
    string CliAuthCode,
    string CliBraId,
    string CliDes,
    string CliOff,
    string CliLingua,
    string CliStatus,
    int[] addIdDevices,
    int[] delIdDevices

) : IRequest<bool>;