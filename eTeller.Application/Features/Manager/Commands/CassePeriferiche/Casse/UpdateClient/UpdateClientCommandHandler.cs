using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Application.Features.Manager.Commands.Users.UpdateUser;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.CassePeriferiche.Casse.UpdateClient;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateClientCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.ClientRepository.GetByIdAsync(request.CliId);
        if (client == null)
        {
            _logger.LogWarning($"Client with ID {request.CliId} not found.");
            throw new NotFoundException($"Client with ID {request.CliId} not found.", null);
        }

        client.CliIp = request.CliIp;
        client.CliMac = request.CliMac;
        client.CliAuthcode = request.CliAuthCode;
        client.CliBraId = request.CliBraId;
        client.CliDes = request.CliDes;
        client.CliOff = request.CliOff;
        client.CliLingua = request.CliLingua;
        client.CliStatus = request.CliStatus;

        _unitOfWork.ClientRepository.UpdateEntity(client);
        await _unitOfWork.Complete();
        return true;
    }
}