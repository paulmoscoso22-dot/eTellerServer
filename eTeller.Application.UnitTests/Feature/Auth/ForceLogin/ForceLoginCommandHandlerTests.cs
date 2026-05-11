using eTeller.Auth.Features.ForceLogin;
using eTeller.Auth.Features.Login;
using eTeller.Auth.ViewModels;
using eTeller.Application.Contracts;
using eTeller.Application.Contracts.Trace;
using eTeller.Domain.Common;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace eTeller.Application.UnitTests.Feature.Auth.ForceLogin;

[Trait("Module", "Auth")]
public class ForceLoginCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IUserSessionRepository> _mockSessionRepo;
    private readonly Mock<ITraceRepository> _mockTraceRepo;
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<ForceLoginCommandHandler>> _mockLogger;
    private readonly ForceLoginCommandHandler _handler;

    public ForceLoginCommandHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockSessionRepo = new Mock<IUserSessionRepository>();
        _mockTraceRepo = new Mock<ITraceRepository>();
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<ForceLoginCommandHandler>>();

        _mockUow.Setup(u => u.UserRepository).Returns(_mockUserRepo.Object);
        _mockUow.Setup(u => u.UserSessionRepository).Returns(_mockSessionRepo.Object);
        _mockUow.Setup(u => u.TraceRepository).Returns(_mockTraceRepo.Object);
        _mockUow.Setup(u => u.Complete()).ReturnsAsync(1);

        _mockTraceRepo.Setup(t => t.InsertTrace(
            It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<bool>())).ReturnsAsync(1);

        _mockSessionRepo.Setup(r => r.TerminateSessionAsync(It.IsAny<string>())).ReturnsAsync(true);

        _handler = new ForceLoginCommandHandler(_mockUow.Object, _mockMediator.Object, _mockLogger.Object);
    }

    private User MakeUser(string? status = null, string? pass = null)
    {
        var enabledStatus = status ?? UserStatusConstants.Enabled;
        var hash = string.IsNullOrEmpty(pass)
            ? BCrypt.Net.BCrypt.HashPassword("Password1!", workFactor: 4)
            : pass;
        return new User
        {
            UsrId = "TESTUSER",
            UsrStatus = enabledStatus,
            UsrPass = hash,
            UsrChgPas = false,
            UsrBraId = "001",
            UsrExtref = "Test User",
            UsrLingua = "IT"
        };
    }

    // ── L-01: Credenziali corrette → sessione terminata e login OK ───────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_CredenzialiCorrette_SessioneTerminataELoginOk()
    {
        // Arrange
        const string plain = "Password1!";
        var hash = BCrypt.Net.BCrypt.HashPassword(plain, workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(MakeUser(pass: hash));
        _mockMediator.Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginVm { ResultCode = "OK", AccessToken = "jwt-token" });
        var command = new ForceLoginCommand("testuser", plain, "192.168.1.1");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockSessionRepo.Verify(r => r.TerminateSessionAsync("TESTUSER"), Times.Once);
        _mockMediator.Verify(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal("OK", result.ResultCode);
    }

    // ── L-02: Credenziali errate → INVALID_CREDENTIALS ──────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_CredenzialiErrate_ReturnInvalidCredentials()
    {
        // Arrange
        var hash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword1!", workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(MakeUser(pass: hash));
        _mockUserRepo.Setup(r => r.IncrementFailedAttemptsAsync(It.IsAny<string>())).ReturnsAsync(1);
        var command = new ForceLoginCommand("TESTUSER", "WrongPassword!", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("INVALID_CREDENTIALS", result.ResultCode);
    }

    // ── L-03: Utente non trovato → INVALID_CREDENTIALS ──────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_UtenteNonTrovato_ReturnInvalidCredentials()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        var command = new ForceLoginCommand("NOUSER", "Password1!", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("INVALID_CREDENTIALS", result.ResultCode);
    }

    // ── L-04: LoginCommand ritorna USER_BLOCKED → propagato ─────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_LoginCommandReturnBlocked_PropagaRisultato()
    {
        // Arrange
        const string plain = "Password1!";
        var hash = BCrypt.Net.BCrypt.HashPassword(plain, workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(MakeUser(pass: hash));
        _mockMediator.Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginVm { ResultCode = "USER_BLOCKED", Message = "Account bloccato." });
        var command = new ForceLoginCommand("TESTUSER", plain, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("USER_BLOCKED", result.ResultCode);
    }

    // ── S-01: Credenziali errate → TerminateSession NON chiamata ─────────────
    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_CredenzialiErrate_SessioneNonTerminata()
    {
        // Arrange - test CRITICO anti session-hijacking
        var hash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword!", workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(MakeUser(pass: hash));
        _mockUserRepo.Setup(r => r.IncrementFailedAttemptsAsync(It.IsAny<string>())).ReturnsAsync(1);
        var command = new ForceLoginCommand("TESTUSER", "WrongPassword!", null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert: TerminateSessionAsync NON deve essere chiamato se credenziali errate
        _mockSessionRepo.Verify(r => r.TerminateSessionAsync(It.IsAny<string>()), Times.Never);
    }

    // ── S-02: SQL Injection in UserId → sessione NON terminata ───────────────
    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_SqlInjectionInUserId_SessioneNonTerminata()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        var command = new ForceLoginCommand("'; DROP TABLE sys_USERS; --", "Password1!", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("INVALID_CREDENTIALS", result.ResultCode);
        _mockSessionRepo.Verify(r => r.TerminateSessionAsync(It.IsAny<string>()), Times.Never);
    }

    // ── S-03: Utente bloccato con credenziali corrette → CreateSession NON chiamata
    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_UtenteBloccatoConCredenzialiCorrette_NessunaSessioneCreata()
    {
        // Arrange: l'utente ha password corretta ma è bloccato.
        // ForceLogin verifica solo la password, NON lo status (lo fa LoginCommand delegato).
        // TerminateSession VIENE chiamata (credenziali corrette), ma CreateSession NON viene
        // chiamata perché LoginCommand delegato ritorna USER_BLOCKED prima di Step 9.
        const string plain = "Password1!";
        var hash = BCrypt.Net.BCrypt.HashPassword(plain, workFactor: 4);
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(MakeUser(status: UserStatusConstants.Blocked, pass: hash));
        _mockMediator.Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginVm { ResultCode = "USER_BLOCKED", Message = "Account bloccato." });
        var command = new ForceLoginCommand("TESTUSER", plain, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("USER_BLOCKED", result.ResultCode);
        _mockSessionRepo.Verify(r => r.CreateSessionAsync(
            It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }
}
