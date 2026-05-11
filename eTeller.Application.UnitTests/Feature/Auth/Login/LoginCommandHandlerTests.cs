using eTeller.Auth.Features.Login;
using eTeller.Application.Contracts.Auth;
using eTeller.Application.Contracts;
using eTeller.Application.Contracts.Trace;
using eTeller.Domain.Common;
using eTeller.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace eTeller.Application.UnitTests.Feature.Auth.Login;

[Trait("Module", "Auth")]
public class LoginCommandHandlerTests
{
    // Mock delle dipendenze condivisi
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IUserSessionRepository> _mockSessionRepo;
    private readonly Mock<ITraceRepository> _mockTraceRepo;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<ILogger<LoginCommandHandler>> _mockLogger;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockSessionRepo = new Mock<IUserSessionRepository>();
        _mockTraceRepo = new Mock<ITraceRepository>();
        _mockTokenService = new Mock<ITokenService>();
        _mockLogger = new Mock<ILogger<LoginCommandHandler>>();

        _mockUow.Setup(u => u.UserRepository).Returns(_mockUserRepo.Object);
        _mockUow.Setup(u => u.UserSessionRepository).Returns(_mockSessionRepo.Object);
        _mockUow.Setup(u => u.TraceRepository).Returns(_mockTraceRepo.Object);
        _mockUow.Setup(u => u.Complete()).ReturnsAsync(1);

        // Default trace mock
        _mockTraceRepo.Setup(t => t.InsertTrace(
            It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<bool>())).ReturnsAsync(1);

        _handler = new LoginCommandHandler(_mockUow.Object, _mockTokenService.Object, _mockLogger.Object);
    }

    // Helper per creare un utente valido di default
    private static User MakeUser(
        string usrId = "TESTUSER",
        string? status = null,  // null = UserStatusConstants.Enabled ("ENABLED")
        string pass = "",       // se vuoto, genera BCrypt di "Password1!"
        bool chgPas = false,
        string braId = "001",
        string extref = "Test User",
        string lingua = "IT") => new User
    {
        UsrId = usrId,
        UsrStatus = status ?? UserStatusConstants.Enabled,
        UsrPass = string.IsNullOrEmpty(pass) ? BCrypt.Net.BCrypt.HashPassword("Password1!", workFactor: 4) : pass,
        UsrChgPas = chgPas,
        UsrBraId = braId,
        UsrExtref = extref,
        UsrLingua = lingua
    };

    // ── L-01: Utente non trovato ─────────────────────────────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_UtenteNonTrovato_ReturnInvalidCredentials()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        var command = new LoginCommand("NOUSER", "anypass", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("INVALID_CREDENTIALS", result.ResultCode);
    }

    // ── L-02: Utente bloccato ────────────────────────────────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_UtenteBloccato_ReturnUserBlocked()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(MakeUser(status: UserStatusConstants.Blocked));
        var command = new LoginCommand("TESTUSER", "anypass", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("USER_BLOCKED", result.ResultCode);
    }

    // ── L-03: Utente non abilitato ───────────────────────────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_UtenteNonAbilitato_ReturnUserDisabled()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(MakeUser(status: "D")); // D = disabled, non A né B
        var command = new LoginCommand("TESTUSER", "anypass", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("USER_DISABLED", result.ResultCode);
    }

    // ── L-04: Password BCrypt errata ─────────────────────────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_PasswordBCryptErrata_ReturnInvalidCredentials()
    {
        // Arrange
        var bcryptHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword1!", workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(MakeUser(pass: bcryptHash));
        _mockUserRepo.Setup(r => r.IncrementFailedAttemptsAsync(It.IsAny<string>())).ReturnsAsync(1);
        var command = new LoginCommand("TESTUSER", "WrongPassword!", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("INVALID_CREDENTIALS", result.ResultCode);
    }

    // ── L-05: Password MD5 errata ────────────────────────────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_PasswordMd5Errata_ReturnInvalidCredentials()
    {
        // Arrange
        // MD5 di "correctpass" in lowercase hex
        var md5Hash = System.Convert.ToHexString(
            System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes("correctpass"))
        ).ToLower();
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(MakeUser(pass: md5Hash));
        _mockUserRepo.Setup(r => r.IncrementFailedAttemptsAsync(It.IsAny<string>())).ReturnsAsync(1);
        var command = new LoginCommand("TESTUSER", "wrongpass", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("INVALID_CREDENTIALS", result.ResultCode);
    }

    // ── L-06: Password MD5 valida → rehash BCrypt ────────────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_PasswordMd5Valida_EsegueMigrazioneRehash()
    {
        // Arrange
        const string plainPassword = "legacypass";
        var md5Hash = System.Convert.ToHexString(
            System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes(plainPassword))
        ).ToLower();
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(MakeUser(pass: md5Hash));
        _mockUserRepo.Setup(r => r.ResetFailedAttemptsAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _mockUserRepo.Setup(r => r.UpdatePasswordAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        _mockSessionRepo.Setup(r => r.GetActiveSessionByUserIdAsync(It.IsAny<string>())).ReturnsAsync((UserSession?)null);
        _mockSessionRepo.Setup(r => r.CreateSessionAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new UserSession { SessionId = 1, UsrId = "TESTUSER", IpAddress = "UNKNOWN", IsActive = true });
        _mockTokenService.Setup(t => t.GenerateToken(It.IsAny<UserTokenClaims>())).Returns("jwt-token");
        _mockTokenService.Setup(t => t.GetTokenExpiration()).Returns(DateTime.UtcNow.AddHours(8));
        var command = new LoginCommand("TESTUSER", plainPassword, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockUserRepo.Verify(r => r.UpdatePasswordAsync(
            It.IsAny<string>(),
            It.Is<string>(h => h.StartsWith("$2"))), // hash BCrypt
            Times.Once);
        Assert.Equal("OK", result.ResultCode);
    }

    // ── L-07: Password BCrypt valida → nessun rehash ─────────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_PasswordBCryptValida_NessunRehash()
    {
        // Arrange
        const string plainPassword = "Password1!";
        var bcryptHash = BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(MakeUser(pass: bcryptHash));
        _mockUserRepo.Setup(r => r.ResetFailedAttemptsAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _mockSessionRepo.Setup(r => r.GetActiveSessionByUserIdAsync(It.IsAny<string>())).ReturnsAsync((UserSession?)null);
        _mockSessionRepo.Setup(r => r.CreateSessionAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new UserSession { SessionId = 1, UsrId = "TESTUSER", IpAddress = "UNKNOWN", IsActive = true });
        _mockTokenService.Setup(t => t.GenerateToken(It.IsAny<UserTokenClaims>())).Returns("jwt-token");
        _mockTokenService.Setup(t => t.GetTokenExpiration()).Returns(DateTime.UtcNow.AddHours(8));
        var command = new LoginCommand("TESTUSER", plainPassword, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockUserRepo.Verify(r => r.UpdatePasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        Assert.Equal("OK", result.ResultCode);
    }

    // ── L-08: Login OK → azzera contatore tentativi ──────────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_LoginOk_AzzeraContatoreTentativi()
    {
        // Arrange
        const string plain = "Password1!";
        var hash = BCrypt.Net.BCrypt.HashPassword(plain, workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(MakeUser(pass: hash));
        _mockUserRepo.Setup(r => r.ResetFailedAttemptsAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _mockSessionRepo.Setup(r => r.GetActiveSessionByUserIdAsync(It.IsAny<string>())).ReturnsAsync((UserSession?)null);
        _mockSessionRepo.Setup(r => r.CreateSessionAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new UserSession { SessionId = 1, UsrId = "TESTUSER", IpAddress = "UNKNOWN", IsActive = true });
        _mockTokenService.Setup(t => t.GenerateToken(It.IsAny<UserTokenClaims>())).Returns("jwt-token");
        _mockTokenService.Setup(t => t.GetTokenExpiration()).Returns(DateTime.UtcNow.AddHours(8));
        var command = new LoginCommand("TESTUSER", plain, null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockUserRepo.Verify(r => r.ResetFailedAttemptsAsync("TESTUSER"), Times.Once);
    }

    // ── L-09: UsrChgPas = true → MUST_CHANGE_PASSWORD ────────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_UsrChgPasTrue_ReturnMustChangePassword()
    {
        // Arrange
        const string plain = "Password1!";
        var hash = BCrypt.Net.BCrypt.HashPassword(plain, workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(MakeUser(pass: hash, chgPas: true));
        _mockUserRepo.Setup(r => r.ResetFailedAttemptsAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        var command = new LoginCommand("TESTUSER", plain, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("MUST_CHANGE_PASSWORD", result.ResultCode);
        Assert.True(result.RequiresPasswordChange);
    }

    // ── L-10: Sessione già attiva → USER_ALREADY_LOGGED ──────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_SessioneGiaAttiva_ReturnUserAlreadyLogged()
    {
        // Arrange
        const string plain = "Password1!";
        var hash = BCrypt.Net.BCrypt.HashPassword(plain, workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(MakeUser(pass: hash));
        _mockUserRepo.Setup(r => r.ResetFailedAttemptsAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _mockSessionRepo.Setup(r => r.GetActiveSessionByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserSession { SessionId = 99, UsrId = "TESTUSER", IpAddress = "10.0.0.1", IsActive = true });
        var command = new LoginCommand("TESTUSER", plain, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("USER_ALREADY_LOGGED", result.ResultCode);
        Assert.True(result.UserAlreadyLogged);
    }

    // ── L-11: Login completo OK → token e sessione ───────────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_LoginCompletoOk_ReturnTokenESessione()
    {
        // Arrange
        const string plain = "Password1!";
        var hash = BCrypt.Net.BCrypt.HashPassword(plain, workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(MakeUser(pass: hash));
        _mockUserRepo.Setup(r => r.ResetFailedAttemptsAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _mockSessionRepo.Setup(r => r.GetActiveSessionByUserIdAsync(It.IsAny<string>())).ReturnsAsync((UserSession?)null);
        _mockSessionRepo.Setup(r => r.CreateSessionAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new UserSession { SessionId = 1, UsrId = "TESTUSER", IpAddress = "192.168.1.1", IsActive = true });
        _mockTokenService.Setup(t => t.GenerateToken(It.IsAny<UserTokenClaims>())).Returns("jwt-access-token");
        _mockTokenService.Setup(t => t.GetTokenExpiration()).Returns(DateTime.UtcNow.AddHours(8));
        var command = new LoginCommand("TESTUSER", plain, "192.168.1.1");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("OK", result.ResultCode);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.UserSession);
        _mockSessionRepo.Verify(r => r.CreateSessionAsync(
            It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
    }

    // ── L-12: Eccezione interna → ERROR senza dettagli ──────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_EccezioneInterna_ReturnErrorSenzaDettagli()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("DB connection lost"));
        var command = new LoginCommand("TESTUSER", "Password1!", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("ERROR", result.ResultCode);
        Assert.DoesNotContain("DB connection lost", result.Message ?? "");
        Assert.DoesNotContain("InvalidOperationException", result.Message ?? "");
    }

    // ── S-01: Anti User Enumeration ──────────────────────────────────────────
    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_UserNotFoundEInvalidCredentials_StessoMessaggioClient()
    {
        // Arrange — scenario 1: utente non trovato
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        var command1 = new LoginCommand("NOUSER", "anypass", null);

        // Act — scenario 1
        var result1 = await _handler.Handle(command1, CancellationToken.None);

        // Arrange — scenario 2: utente trovato ma password BCrypt errata
        var bcryptHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword1!", workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(MakeUser(pass: bcryptHash));
        _mockUserRepo.Setup(r => r.IncrementFailedAttemptsAsync(It.IsAny<string>())).ReturnsAsync(1);
        var command2 = new LoginCommand("TESTUSER", "WrongPassword!", null);

        // Act — scenario 2
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        // Assert — i messaggi DEVONO essere identici per prevenire user enumeration
        Assert.Equal(result1.Message, result2.Message);
    }

    // ── S-02: SQL Injection in UserId ────────────────────────────────────────
    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_SqlInjectionInUserId_ReturnInvalidCredentials()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        var command = new LoginCommand("'; DROP TABLE sys_USERS; --", "anypass", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("INVALID_CREDENTIALS", result.ResultCode);
    }

    // ── S-03: SQL Injection in Password ─────────────────────────────────────
    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_SqlInjectionInPassword_ReturnInvalidCredentials()
    {
        // Arrange
        var bcryptHash = BCrypt.Net.BCrypt.HashPassword("RealPassword1!", workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(MakeUser(pass: bcryptHash));
        _mockUserRepo.Setup(r => r.IncrementFailedAttemptsAsync(It.IsAny<string>())).ReturnsAsync(1);
        var command = new LoginCommand("TESTUSER", "' OR '1'='1", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("INVALID_CREDENTIALS", result.ResultCode);
    }

    // ── S-04: UserId lunghissimo (DoS / Buffer Overflow) ─────────────────────
    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_UserIdLunghissimo_NonCrasha()
    {
        // Arrange
        var longUserId = new string('A', 1000);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        var command = new LoginCommand(longUserId, "anypass", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("INVALID_CREDENTIALS", result.ResultCode);
    }

    // ── S-05: Brute Force N-1 → utente ancora attivo ─────────────────────────
    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_BruteForce_SogliaMenoUno_UtenteAncoraAttivo()
    {
        // Arrange
        var bcryptHash = BCrypt.Net.BCrypt.HashPassword("RealPassword1!", workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(MakeUser(status: UserStatusConstants.Enabled, pass: bcryptHash));
        _mockUserRepo.Setup(r => r.IncrementFailedAttemptsAsync(It.IsAny<string>()))
            .ReturnsAsync(PasswordPolicyConstants.MaxFailedAttempts - 1);
        var command = new LoginCommand("TESTUSER", "WrongPassword!", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert — l'handler deve rispondere INVALID_CREDENTIALS (non USER_BLOCKED)
        // Il blocco viene gestito internamente dal repository solo al raggiungimento della soglia
        Assert.Equal("INVALID_CREDENTIALS", result.ResultCode);
    }

    // ── S-06: Brute Force N → utente bloccato ────────────────────────────────
    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_BruteForce_SogliaRaggiunta_UtenteBloccato()
    {
        // Arrange — prima chiamata: utente Enabled, password errata, Increment raggiunge la soglia
        var bcryptHash = BCrypt.Net.BCrypt.HashPassword("RealPassword1!", workFactor: 4);
        _mockUserRepo.SetupSequence(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(MakeUser(status: UserStatusConstants.Enabled, pass: bcryptHash))
            .ReturnsAsync(MakeUser(status: UserStatusConstants.Blocked, pass: bcryptHash));
        _mockUserRepo.Setup(r => r.IncrementFailedAttemptsAsync(It.IsAny<string>()))
            .ReturnsAsync(PasswordPolicyConstants.MaxFailedAttempts);

        var commandFirst = new LoginCommand("TESTUSER", "WrongPassword!", null);
        await _handler.Handle(commandFirst, CancellationToken.None);

        // Act — seconda chiamata: il repository ha bloccato l'utente internamente
        var commandSecond = new LoginCommand("TESTUSER", "WrongPassword!", null);
        var result = await _handler.Handle(commandSecond, CancellationToken.None);

        // Assert — l'utente ora risulta bloccato
        Assert.Equal("USER_BLOCKED", result.ResultCode);
    }

    // ── S-07: UserId solo spazi → non bypassa auth ───────────────────────────
    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_UserIdSoloSpazi_NonBypassaAuth()
    {
        // Arrange — dopo Trim → "" → ToUpperInvariant → ""
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        var command = new LoginCommand("   ", "anypass", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("INVALID_CREDENTIALS", result.ResultCode);
    }

    // ── S-08: Password vuota → non bypassa verifica ──────────────────────────
    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_PasswordVuota_NonBypassaVerifica()
    {
        // Arrange
        var bcryptHash = BCrypt.Net.BCrypt.HashPassword("RealPassword1!", workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(MakeUser(pass: bcryptHash));
        _mockUserRepo.Setup(r => r.IncrementFailedAttemptsAsync(It.IsAny<string>())).ReturnsAsync(1);
        var command = new LoginCommand("TESTUSER", "", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert — BCrypt.Verify("", hash) → false: la verifica NON è stata saltata
        Assert.Equal("INVALID_CREDENTIALS", result.ResultCode);
        _mockUserRepo.Verify(r => r.IncrementFailedAttemptsAsync(It.IsAny<string>()), Times.Once);
    }
}
