using System.Security.Cryptography;
using System.Text;
using eTeller.Application.Contracts;
using eTeller.Auth.Features.Login;
using eTeller.Auth.ViewModels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Auth.Features.ForceLogin;

public class ForceLoginCommandHandler : IRequestHandler<ForceLoginCommand, LoginVm>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<ForceLoginCommandHandler> _logger;

    public ForceLoginCommandHandler(
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<ForceLoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<LoginVm> Handle(ForceLoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(ForceLoginCommand));

        var userId = request.UserId.Trim().ToUpperInvariant();
        var ipAddress = request.CashDeskIp ?? "UNKNOWN";

        // Verifica credenziali PRIMA di qualsiasi side-effect (anti session-hijacking).
        // Un attaccante che conosce solo uno username NON deve poter disconnettere utenti legittimi.
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user is null || !VerifyPassword(request.Password, user.UsrPass))
        {
            if (user is not null)
                await _unitOfWork.UserRepository.IncrementFailedAttemptsAsync(userId);

            _logger.LogWarning("ForceLogin fallito — credenziali non valide: {UserId}", userId);
            await _unitOfWork.Complete();
            return new LoginVm { ResultCode = "INVALID_CREDENTIALS", Message = "Credenziali non valide." };
        }

        // Termina la sessione esistente dell'utente (credenziali verificate)
        await _unitOfWork.UserSessionRepository.TerminateSessionAsync(userId);

        // Trace force login
        await _unitOfWork.TraceRepository.InsertTrace(
            traTime: DateTime.Now,
            traUser: userId,
            traFunCode: "FORCE_LOGIN",
            traSubFun: null,
            traStation: ipAddress,
            traTabNam: "sys_USERS",
            traEntCode: userId,
            traRevTrxTrace: null,
            traDes: "Sessione esistente terminata da force-login",
            traExtRef: null,
            traError: false);

        await _unitOfWork.Complete();

        // Delega a LoginCommand — la sessione è già terminata, il check USER_ALREADY_LOGGED passerà
        _logger.LogInformation("Handled {Command} — delegando a LoginCommand per {UserId}",
            nameof(ForceLoginCommand), userId);

        return await _mediator.Send(
            new LoginCommand(request.UserId, request.Password, request.CashDeskIp),
            cancellationToken);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifica preliminare credenziali (BCrypt-first, MD5 fallback).
    /// Usata per bloccare il side-effect di TerminateSession prima del controllo reale in LoginCommand.
    /// </summary>
    private static bool VerifyPassword(string plainPassword, string storedHash)
    {
        if (storedHash.StartsWith("$2", StringComparison.Ordinal))
            return BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);

        var md5Hash = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(plainPassword))).ToLower();
        return string.Equals(md5Hash, storedHash, StringComparison.Ordinal);
    }
}
