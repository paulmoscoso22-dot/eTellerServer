using eTeller.Application.Contracts;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.GestioneErrori.Commands.UpdateGestioneErrori;

public class UpdateGestioneErroriCommandHandler : IRequestHandler<UpdateGestioneErroriCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateGestioneErroriCommandHandler> _logger;

    public UpdateGestioneErroriCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateGestioneErroriCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateGestioneErroriCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {CommandName} ErrId:{ErrId}", nameof(UpdateGestioneErroriCommand), request.ErrId);

        var errorCode = new ErrorCode
        {
            ErrId      = request.ErrId,
            ErrTyp     = request.ErrTyp,
            ErrCanFlag = request.ErrCanFlag,
            ErrConFlag = request.ErrConFlag,
            ErrForFlag = request.ErrForFlag,
            ErrFocId   = request.ErrFocId,
            ErrDesSol  = request.ErrDesSol,
            ErrDescIt  = request.ErrDescIt,
            ErrDescEn  = request.ErrDescEn,
            ErrDescFr  = request.ErrDescFr,
            ErrDescDe  = request.ErrDescDe
        };

        _unitOfWork.Repository<ErrorCode>().UpdateEntity(errorCode);
        await _unitOfWork.Complete();
        return true;
    }
}
