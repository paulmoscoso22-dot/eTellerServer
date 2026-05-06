using eTeller.Application.Contracts;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.GestioneErrori.Commands.InsertGestioneErrori;

public class InsertGestioneErroriCommandHandler : IRequestHandler<InsertGestioneErroriCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InsertGestioneErroriCommandHandler> _logger;

    public InsertGestioneErroriCommandHandler(IUnitOfWork unitOfWork, ILogger<InsertGestioneErroriCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(InsertGestioneErroriCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {CommandName} ErrId:{ErrId}", nameof(InsertGestioneErroriCommand), request.ErrId);

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

        _unitOfWork.Repository<ErrorCode>().AddEntity(errorCode);
        await _unitOfWork.Complete();
        return Unit.Value;
    }
}
