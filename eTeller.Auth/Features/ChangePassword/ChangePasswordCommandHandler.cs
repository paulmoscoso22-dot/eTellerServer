using System.Security.Cryptography;
using System.Text;
using eTeller.Application.Contracts;
using eTeller.Application.Contracts.Auth;
using eTeller.Auth.ViewModels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Auth.Features.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordVm>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHistoryRepository _passwordHistoryRepository;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHistoryRepository passwordHistoryRepository,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHistoryRepository = passwordHistoryRepository;
        _logger = logger;
    }

    public async Task<ChangePasswordVm> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(ChangePasswordCommand));

        var userId = request.UserId.Trim().ToUpperInvariant();
        var station = request.TraStation;

        try
        {
            // Step 1: Recupero utente
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user is null)
            {
                return new ChangePasswordVm { ResultCode = "ERROR", Message = "Utente non trovato." };
            }

            // Step 2: Verifica password corrente (BCrypt-first, MD5 fallback)
            bool currentValid = VerifyPassword(request.CurrentPassword, user.UsrPass);
            if (!currentValid)
            {
                _logger.LogWarning("ChangePassword fallito — password corrente errata per {UserId}", userId);
                await InsertTrace(userId, station, "CHANGE_PASSWORD", "Password corrente non valida", isError: true);
                await _unitOfWork.Complete();
                return new ChangePasswordVm
                {
                    ResultCode = "INVALID_CURRENT_PASSWORD",
                    Message = "La password corrente non è corretta."
                };
            }

            // Step 3: Leggo PasswordHistoryLimit da PERSONALISATION
            int historyLimit = await GetPasswordHistoryLimitAsync();

            // Step 4: Verifica storico password
            var newPasswordBcrypt = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);
            bool inHistory = await _passwordHistoryRepository.IsPasswordInHistoryAsync(
                userId, request.NewPassword, historyLimit);

            if (inHistory)
            {
                _logger.LogWarning("ChangePassword fallito — password già usata in storico per {UserId}", userId);
                await InsertTrace(userId, station, "CHANGE_PASSWORD",
                    $"Nuova password già usata nelle ultime {historyLimit} password", isError: true);
                await _unitOfWork.Complete();
                return new ChangePasswordVm
                {
                    ResultCode = "HISTORY_VIOLATION",
                    Message = $"La nuova password deve essere diversa dalle ultime {historyLimit} password utilizzate."
                };
            }

            // Step 5: Inserisce nuova password in storico
            await _passwordHistoryRepository.InsertPasswordAsync(userId, newPasswordBcrypt, DateTime.Now);

            // Step 6: Pulisce storico oltre il limite
            await _passwordHistoryRepository.ClearOldPasswordsAsync(userId, historyLimit);

            // Step 7: Aggiorna hash su sys_USERS e resetta flag USR_CHG_PAS
            await _unitOfWork.UserRepository.UpdatePasswordAsync(userId, newPasswordBcrypt);

            // Step 8: Trace OK
            await InsertTrace(userId, station, "CHANGE_PASSWORD", "Cambio password completato", isError: false);
            await _unitOfWork.Complete();

            _logger.LogInformation("Handled {Command} — password aggiornata: {UserId}", nameof(ChangePasswordCommand), userId);

            return new ChangePasswordVm { ResultCode = "OK", Message = "Password aggiornata con successo." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore imprevisto durante il cambio password per {UserId}", userId);
            return new ChangePasswordVm { ResultCode = "ERROR", Message = "Errore interno. Riprovare più tardi." };
        }
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static bool VerifyPassword(string plainPassword, string storedHash)
    {
        if (storedHash.StartsWith("$2", StringComparison.Ordinal))
            return BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);

        // MD5 legacy fallback
        var md5Hash = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(plainPassword))).ToLower();
        return string.Equals(md5Hash, storedHash, StringComparison.Ordinal);
    }

    private async Task<int> GetPasswordHistoryLimitAsync()
    {
        var results = await _unitOfWork.PersonalisationRepository
            .GetAsync(p => p.ParId == "PasswordHistoryLimit");
        var param = results.FirstOrDefault();
        if (param is not null && int.TryParse(param.ParValue, out var limit))
            return limit;
        return 5; // default sicuro
    }

    private Task InsertTrace(string userId, string station, string funCode, string? description, bool isError)
        => _unitOfWork.TraceRepository.InsertTrace(
            traTime: DateTime.Now,
            traUser: userId,
            traFunCode: funCode,
            traSubFun: null,
            traStation: station,
            traTabNam: "sys_USERS",
            traEntCode: userId,
            traRevTrxTrace: null,
            traDes: description,
            traExtRef: null,
            traError: isError);
}
