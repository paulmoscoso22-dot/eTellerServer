using MediatR;
using eTeller.Application.Contracts;
using eTeller.Domain.Models;

namespace eTeller.Application.Features.CassePeriferiche.Commands;

public class InsertClientDeviceCommandHandler : IRequestHandler<InsertClientDeviceCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public InsertClientDeviceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(InsertClientDeviceCommand request, CancellationToken cancellationToken)
    {
        var clientDevice = new Client_Device
        {
            CliId = request.CliId,
            DevId = request.DevId
        };

        _unitOfWork.ClientDeviceRepository.AddEntity(clientDevice);
        await _unitOfWork.Complete();
        return true;
    }
}