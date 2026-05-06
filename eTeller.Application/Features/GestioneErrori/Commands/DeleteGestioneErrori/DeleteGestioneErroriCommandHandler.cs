using eTeller.Application.Contracts;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.GestioneErrori.Commands.DeleteGestioneErrori;

public class DeleteGestioneErroriCommandHandler : IRequestHandler<DeleteGestioneErroriCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteGestioneErroriCommandHandler> _logger;

    public DeleteGestioneErroriCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteGestioneErroriCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteGestioneErroriCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {CommandName} ErrId:{ErrId}", nameof(DeleteGestioneErroriCommand), request.ErrId);

        var results = await _unitOfWork.Repository<ErrorCode>().GetAsync(e => e.ErrId == request.ErrId);
        var entity = results.FirstOrDefault();
        if (entity is null)
            return false;

        _unitOfWork.Repository<ErrorCode>().DeleteEntity(entity);
        await _unitOfWork.Complete();
        return true;
    }
}
