using MediatR;

namespace eTeller.Application.Features.CassePeriferiche.Commands;

public record InsertClientDeviceCommand(
    string CliId,
    int DevId
) : IRequest<bool>;