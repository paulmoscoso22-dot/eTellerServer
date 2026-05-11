using eTeller.Application.Contracts;
using eTeller.Auth.ViewModels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Auth.Features.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutVm>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<LogoutCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<LogoutVm> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(LogoutCommand));

        var userId = request.UserId.Trim().ToUpperInvariant();

        try
        {
            // Chiude la sessione attiva dell'utente (sys_USER_SESSIONS)
            await _unitOfWork.UserSessionRepository.TerminateSessionAsync(userId);

            // Trace logout
            await _unitOfWork.TraceRepository.InsertTrace(
                traTime: DateTime.Now,
                traUser: userId,
                traFunCode: "LOGOUT",
                traSubFun: null,
                traStation: request.TraStation,
                traTabNam: "sys_USERS",
                traEntCode: userId,
                traRevTrxTrace: null,
                traDes: "Logout completato",
                traExtRef: null,
                traError: false);

            await _unitOfWork.Complete();

            _logger.LogInformation("Handled {Command} — logout completato: {UserId}", nameof(LogoutCommand), userId);

            return new LogoutVm { ResultCode = "OK", Message = "Logout completato." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore imprevisto durante il logout per {UserId}", userId);
            return new LogoutVm { ResultCode = "ERROR", Message = "Errore interno durante il logout." };
        }
    }
}
