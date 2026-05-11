using eTeller.Application.Contracts;
using eTeller.Application.Contracts.Trace;
using eTeller.Auth.Features.Logout;
using eTeller.Auth.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;

namespace eTeller.Application.UnitTests.Feature.Auth.Logout;

[Trait("Module", "Auth")]
public class LogoutCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IUserSessionRepository> _mockSessionRepo;
    private readonly Mock<ITraceRepository> _mockTraceRepo;
    private readonly Mock<ILogger<LogoutCommandHandler>> _mockLogger;
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockSessionRepo = new Mock<IUserSessionRepository>();
        _mockTraceRepo = new Mock<ITraceRepository>();
        _mockLogger = new Mock<ILogger<LogoutCommandHandler>>();

        _mockUow.Setup(u => u.UserSessionRepository).Returns(_mockSessionRepo.Object);
        _mockUow.Setup(u => u.TraceRepository).Returns(_mockTraceRepo.Object);
        _mockUow.Setup(u => u.Complete()).ReturnsAsync(1);

        _mockTraceRepo.Setup(t => t.InsertTrace(
            It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<bool>())).ReturnsAsync(1);

        _mockSessionRepo.Setup(r => r.TerminateSessionAsync(It.IsAny<string>())).ReturnsAsync(true);

        _handler = new LogoutCommandHandler(_mockUow.Object, _mockLogger.Object);
    }

    // ── L-01: Logout OK → sessione terminata e trace inserita ───────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_LogoutOk_SessioneTerminataETraceInserita()
    {
        // Arrange
        var command = new LogoutCommand("abc", "TESTUSER", "192.168.1.1");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("OK", result.ResultCode);
        _mockSessionRepo.Verify(r => r.TerminateSessionAsync("TESTUSER"), Times.Once);
        _mockTraceRepo.Verify(t => t.InsertTrace(
            It.IsAny<DateTime>(), It.IsAny<string>(),
            It.Is<string>(f => f == "LOGOUT"),
            It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<bool>()), Times.Once);
    }

    // ── L-02: Sessione non esistente → comportamento idempotente ────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_SessioneNonEsistente_IdempotenteSenzaErrore()
    {
        // Arrange — TerminateSessionAsync returns false (sessione non trovata) senza lanciare eccezione
        _mockSessionRepo.Setup(r => r.TerminateSessionAsync(It.IsAny<string>())).ReturnsAsync(false);
        var command = new LogoutCommand("abc", "TESTUSER", "192.168.1.1");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("OK", result.ResultCode);
        _mockSessionRepo.Verify(r => r.TerminateSessionAsync(It.IsAny<string>()), Times.Once);
    }

    // ── L-03: Eccezione interna → ERROR ─────────────────────────────────────
    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_EccezioneInterna_ReturnError()
    {
        // Arrange
        _mockSessionRepo.Setup(r => r.TerminateSessionAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("DB error"));
        var command = new LogoutCommand("abc", "TESTUSER", "192.168.1.1");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("ERROR", result.ResultCode);
    }

    // ── S-01: UserId viene sanitizzato (ToUpperInvariant) ───────────────────
    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_UserIdSanitizzato_ToUpperInvariantApplicato()
    {
        // Arrange
        var command = new LogoutCommand("abc", "testuser", "192.168.1.1");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert — TerminateSessionAsync deve ricevere "TESTUSER" (uppercase)
        _mockSessionRepo.Verify(r => r.TerminateSessionAsync(
            It.Is<string>(s => s == "TESTUSER")), Times.Once);
    }
}
