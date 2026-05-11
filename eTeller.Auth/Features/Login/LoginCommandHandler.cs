using System.Security.Cryptography;
using System.Text;
using eTeller.Application.Contracts;
using eTeller.Application.Contracts.Auth;
using eTeller.Auth.ViewModels;
using eTeller.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Auth.Features.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginVm>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginVm> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(LoginCommand));

        var userId = SanitizeInput(request.UserId);
        var ipAddress = request.CashDeskIp ?? "UNKNOWN";

        try
        {
            // Step 1: Recupero utente (anti user-enumeration: USER_NOT_FOUND = stesso messaggio di INVALID_CREDENTIALS)
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user is null)
            {
                _logger.LogWarning("Login fallito — utente non trovato: {UserId}", userId);
                await InsertTrace(userId, ipAddress, "LOGIN_FAILED", "Utente non trovato", true);
                await _unitOfWork.Complete();
                return GenericInvalidCredentials();
            }

            // Step 2: Utente bloccato
            if (user.UsrStatus == UserStatusConstants.Blocked)
            {
                _logger.LogWarning("Login fallito — utente bloccato: {UserId}", userId);
                await InsertTrace(userId, ipAddress, "LOGIN_FAILED", "Utente bloccato", true);
                await _unitOfWork.Complete();
                return new LoginVm
                {
                    ResultCode = "USER_BLOCKED",
                    Message = "Account bloccato. Contattare l'amministratore di sistema."
                };
            }

            // Step 3: Utente non abilitato
            if (user.UsrStatus != UserStatusConstants.Enabled)
            {
                _logger.LogWarning("Login fallito — utente non abilitato: {UserId}", userId);
                await InsertTrace(userId, ipAddress, "LOGIN_FAILED", "Utente non abilitato", true);
                await _unitOfWork.Complete();
                return new LoginVm
                {
                    ResultCode = "USER_DISABLED",
                    Message = "Account non abilitato. Contattare l'amministratore di sistema."
                };
            }

            // Step 4: Verifica password — BCrypt first, fallback MD5 legacy con rehash automatico
            bool passwordValid = VerifyPassword(request.Password, user.UsrPass, out bool needsRehash);
            if (!passwordValid)
            {
                var attempts = await _unitOfWork.UserRepository.IncrementFailedAttemptsAsync(userId);
                _logger.LogWarning("Login fallito — password errata: {UserId}, tentativo {Attempt}", userId, attempts);
                await InsertTrace(userId, ipAddress, "LOGIN_FAILED", $"Password errata (tentativo {attempts})", true);
                await _unitOfWork.Complete();
                return GenericInvalidCredentials();
            }

            // Step 5: Lazy migration MD5 → BCrypt
            if (needsRehash)
            {
                var bcryptHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);
                await _unitOfWork.UserRepository.UpdatePasswordAsync(userId, bcryptHash);
                _logger.LogInformation("Password migrata da MD5 a BCrypt per utente {UserId}", userId);
            }

            // Step 6: Reset tentativi falliti (login con credenziali corrette)
            await _unitOfWork.UserRepository.ResetFailedAttemptsAsync(userId);

            // Step 7: Cambio password obbligatorio
            if (user.UsrChgPas)
            {
                await InsertTrace(userId, ipAddress, "LOGIN_FAILED", "Cambio password obbligatorio", false);
                await _unitOfWork.Complete();
                return new LoginVm
                {
                    ResultCode = "MUST_CHANGE_PASSWORD",
                    Message = "È necessario cambiare la password prima di accedere.",
                    RequiresPasswordChange = true
                };
            }

            // Step 8: Sessione già attiva da altro client
            var existingSession = await _unitOfWork.UserSessionRepository.GetActiveSessionByUserIdAsync(userId);
            if (existingSession is not null)
            {
                _logger.LogInformation("Utente {UserId} già connesso — sessione attiva trovata", userId);
                await InsertTrace(userId, ipAddress, "LOGIN_FAILED", "Sessione già attiva", false);
                await _unitOfWork.Complete();
                return new LoginVm
                {
                    ResultCode = "USER_ALREADY_LOGGED",
                    Message = "Utente già connesso da un'altra postazione. Usare force-login per disconnettere la sessione esistente.",
                    UserAlreadyLogged = true
                };
            }

            // Step 9: Crea nuova sessione
            var session = await _unitOfWork.UserSessionRepository.CreateSessionAsync(
                userId, cliId: null, ipAddress, forcedLogin: false);

            // Step 10: Genera token JWT
            var sessionId = session?.SessionId.ToString() ?? Guid.NewGuid().ToString();
            var tokenClaims = new UserTokenClaims(
                UserId: userId,
                UserName: user.UsrExtref ?? userId,
                BranchId: user.UsrBraId,
                Language: user.UsrLingua,
                CanUseTeller: false,
                CashDeskId: null,
                CashDeskBranchId: null,
                IpAddress: ipAddress,
                SessionId: sessionId);

            var accessToken = _tokenService.GenerateToken(tokenClaims);
            var tokenExpiry = _tokenService.GetTokenExpiration();

            // Step 11: Trace login OK
            await InsertTrace(userId, ipAddress, "LOGIN", "Login completato", false);
            await _unitOfWork.Complete();

            _logger.LogInformation("Handled {Command} — login completato: {UserId}", nameof(LoginCommand), userId);

            return new LoginVm
            {
                ResultCode = "OK",
                Message = "Login completato.",
                AccessToken = accessToken,
                TokenExpiresAt = tokenExpiry,
                UserSession = new LoginSessionVm
                {
                    UserId = userId,
                    UserName = user.UsrExtref ?? userId,
                    BranchId = user.UsrBraId,
                    Language = user.UsrLingua,
                    CanUseTeller = false
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore imprevisto durante il login per {UserId}", userId);
            return new LoginVm { ResultCode = "ERROR", Message = "Errore interno. Riprovare più tardi." };
        }
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static string SanitizeInput(string input)
        => input.Trim().ToUpperInvariant();

    /// <summary>
    /// Verifica la password con strategia BCrypt-first, MD5-fallback.
    /// Se la password è salvata in MD5, imposta needsRehash=true per la lazy migration.
    /// </summary>
    private static bool VerifyPassword(string plainPassword, string storedHash, out bool needsRehash)
    {
        needsRehash = false;

        // BCrypt hashes iniziano con $2a$, $2b$, $2x$ o $2y$
        if (storedHash.StartsWith("$2", StringComparison.Ordinal))
            return BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);

        // Fallback MD5 legacy (ClassCifrarePass.CifraPass — UTF8, lowercase hex)
        var md5Hash = ComputeMd5LowercaseHex(plainPassword);
        if (string.Equals(md5Hash, storedHash, StringComparison.Ordinal))
        {
            needsRehash = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Replica esatta di ClassCifrarePass.CifraPass (eTeller2022 legacy).
    /// MD5(UTF8 bytes) → lowercase hex string.
    /// </summary>
    private static string ComputeMd5LowercaseHex(string input)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }

    /// <summary>
    /// Risposta generica per credenziali non valide (anti user-enumeration).
    /// Non distingue USER_NOT_FOUND da INVALID_CREDENTIALS verso il client.
    /// </summary>
    private static LoginVm GenericInvalidCredentials() =>
        new LoginVm { ResultCode = "INVALID_CREDENTIALS", Message = "Credenziali non valide." };

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
