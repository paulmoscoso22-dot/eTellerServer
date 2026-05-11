using System.Linq.Expressions;
using eTeller.Application.Contracts;
using eTeller.Application.Contracts.Auth;
using eTeller.Application.Contracts.Personalisation;
using eTeller.Application.Contracts.Trace;
using eTeller.Auth.Features.ChangePassword;
using eTeller.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace eTeller.Application.UnitTests.Feature.Auth.ChangePassword;

[Trait("Module", "Auth")]
public class ChangePasswordCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IPersonalisationRepository> _mockPersonalisationRepo;
    private readonly Mock<IPasswordHistoryRepository> _mockPasswordHistoryRepo;
    private readonly Mock<ITraceRepository> _mockTraceRepo;
    private readonly Mock<ILogger<ChangePasswordCommandHandler>> _mockLogger;
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockPersonalisationRepo = new Mock<IPersonalisationRepository>();
        _mockPasswordHistoryRepo = new Mock<IPasswordHistoryRepository>();
        _mockTraceRepo = new Mock<ITraceRepository>();
        _mockLogger = new Mock<ILogger<ChangePasswordCommandHandler>>();

        _mockUow.Setup(u => u.UserRepository).Returns(_mockUserRepo.Object);
        _mockUow.Setup(u => u.PersonalisationRepository).Returns(_mockPersonalisationRepo.Object);
        _mockUow.Setup(u => u.TraceRepository).Returns(_mockTraceRepo.Object);
        _mockUow.Setup(u => u.Complete()).ReturnsAsync(1);

        // Default: PasswordHistoryLimit = 5 (not configured → default)
        _mockPersonalisationRepo
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Personalisation, bool>>>()))
            .ReturnsAsync(Array.Empty<Personalisation>());

        _mockTraceRepo.Setup(t => t.InsertTrace(
            It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<bool>())).ReturnsAsync(1);

        _mockUserRepo.Setup(r => r.UpdatePasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _mockPasswordHistoryRepo.Setup(r => r.InsertPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
            .Returns(Task.CompletedTask);

        _mockPasswordHistoryRepo.Setup(r => r.ClearOldPasswordsAsync(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        _handler = new ChangePasswordCommandHandler(
            _mockUow.Object,
            _mockPasswordHistoryRepo.Object,
            _mockLogger.Object);
    }

    private static User MakeUser(string plainPassword) => new User
    {
        UsrId = "TESTUSER",
        UsrPass = BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 4),
        UsrStatus = Domain.Common.UserStatusConstants.Enabled,
        UsrBraId = "001",
        UsrLingua = "IT",
        UsrChgPas = false,
        UsrTentativi = 0
    };

    // ── L-01: Cambio password OK ────────────────────────────────────────────

    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_CambioPasswordOk_HashBCryptSalvato()
    {
        var user = MakeUser("CurrentPass123!");
        _mockUserRepo.Setup(r => r.GetByIdAsync("TESTUSER")).ReturnsAsync(user);
        _mockPasswordHistoryRepo
            .Setup(r => r.IsPasswordInHistoryAsync("TESTUSER", "NewPass456!", It.IsAny<int>()))
            .ReturnsAsync(false);

        var command = new ChangePasswordCommand(
            UserId: "testuser",
            CurrentPassword: "CurrentPass123!",
            NewPassword: "NewPass456!",
            TraStation: "192.168.1.1");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("OK", result.ResultCode);

        _mockUserRepo.Verify(r => r.UpdatePasswordAsync(
            It.Is<string>(id => id == "TESTUSER"),
            It.Is<string>(h => h.StartsWith("$2"))),
            Times.Once);

        _mockPasswordHistoryRepo.Verify(r => r.InsertPasswordAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
            Times.Once);

        _mockUow.Verify(u => u.Complete(), Times.AtLeastOnce);
    }

    // ── L-02: Utente non trovato ────────────────────────────────────────────

    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_UtenteNonTrovato_ReturnError()
    {
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var command = new ChangePasswordCommand("anyuser", "pass", "newpass", "127.0.0.1");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("ERROR", result.ResultCode);
        Assert.Contains("non trovato", result.Message, StringComparison.OrdinalIgnoreCase);

        _mockUserRepo.Verify(r => r.UpdatePasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    // ── L-03: Password corrente errata ─────────────────────────────────────

    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_PasswordCorrenteErrata_ReturnInvalidCurrentPassword()
    {
        var user = MakeUser("CorrectPass123!");
        _mockUserRepo.Setup(r => r.GetByIdAsync("TESTUSER")).ReturnsAsync(user);

        var command = new ChangePasswordCommand("testuser", "WrongPassword!", "NewPass456!", "127.0.0.1");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("INVALID_CURRENT_PASSWORD", result.ResultCode);

        _mockUserRepo.Verify(r => r.UpdatePasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    // ── L-04: Nuova password uguale alla corrente → HISTORY_VIOLATION ───────

    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_NuovaPasswordUgualeCorrente_ReturnHistoryViolation()
    {
        var user = MakeUser("CurrentPass123!");
        _mockUserRepo.Setup(r => r.GetByIdAsync("TESTUSER")).ReturnsAsync(user);
        _mockPasswordHistoryRepo
            .Setup(r => r.IsPasswordInHistoryAsync("TESTUSER", "CurrentPass123!", It.IsAny<int>()))
            .ReturnsAsync(true);

        var command = new ChangePasswordCommand("testuser", "CurrentPass123!", "CurrentPass123!", "127.0.0.1");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("HISTORY_VIOLATION", result.ResultCode);

        _mockUserRepo.Verify(r => r.UpdatePasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    // ── L-05: Nuova password in storico (diversa ma già usata) ─────────────

    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_NuovaPasswordInStorico_ReturnHistoryViolation()
    {
        var user = MakeUser("CurrentPass123!");
        _mockUserRepo.Setup(r => r.GetByIdAsync("TESTUSER")).ReturnsAsync(user);

        _mockPersonalisationRepo
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Personalisation, bool>>>()))
            .ReturnsAsync(new List<Personalisation>
            {
                new() { ParId = "PasswordHistoryLimit", ParValue = "3" }
            });

        _mockPasswordHistoryRepo
            .Setup(r => r.IsPasswordInHistoryAsync("TESTUSER", "OldUsedPass!", It.IsAny<int>()))
            .ReturnsAsync(true);

        var command = new ChangePasswordCommand("testuser", "CurrentPass123!", "OldUsedPass!", "127.0.0.1");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("HISTORY_VIOLATION", result.ResultCode);
        Assert.Contains("3", result.Message);
    }

    // ── L-06: Eccezione interna → ERROR ────────────────────────────────────

    [Fact]
    [Trait("Category", "FlowLegacy")]
    public async Task Handle_EccezioneInterna_ReturnError()
    {
        _mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("DB down"));

        var command = new ChangePasswordCommand("testuser", "pass", "newpass", "127.0.0.1");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("ERROR", result.ResultCode);
    }

    // ── S-01: Password non appare nei log ──────────────────────────────────

    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_PasswordNonAppareNeiLog_MockLoggerVerificato()
    {
        var user = MakeUser("CurrentPass123!");
        _mockUserRepo.Setup(r => r.GetByIdAsync("TESTUSER")).ReturnsAsync(user);
        _mockPasswordHistoryRepo
            .Setup(r => r.IsPasswordInHistoryAsync("TESTUSER", "NewPass456!", It.IsAny<int>()))
            .ReturnsAsync(false);

        var command = new ChangePasswordCommand("testuser", "CurrentPass123!", "NewPass456!", "192.168.1.1");

        await _handler.Handle(command, CancellationToken.None);

        _mockLogger.Verify(l => l.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) =>
                v.ToString()!.Contains("CurrentPass123!") ||
                v.ToString()!.Contains("NewPass456!")),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    // ── S-02: Nuova password salvata come hash BCrypt (non plain text) ──────

    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_NuovaPasswordSalvataHashBCrypt_NonPlainText()
    {
        var user = MakeUser("CurrentPass123!");
        _mockUserRepo.Setup(r => r.GetByIdAsync("TESTUSER")).ReturnsAsync(user);
        _mockPasswordHistoryRepo
            .Setup(r => r.IsPasswordInHistoryAsync("TESTUSER", "NewPass456!", It.IsAny<int>()))
            .ReturnsAsync(false);

        var command = new ChangePasswordCommand("testuser", "CurrentPass123!", "NewPass456!", "192.168.1.1");

        await _handler.Handle(command, CancellationToken.None);

        _mockUserRepo.Verify(r => r.UpdatePasswordAsync(
            It.IsAny<string>(),
            It.Is<string>(h => h.StartsWith("$2"))),
            Times.Once);

        _mockUserRepo.Verify(r => r.UpdatePasswordAsync(
            It.IsAny<string>(),
            "NewPass456!"),
            Times.Never);
    }

    // ── S-03: Storico limite esatto → password rifiutata (off-by-one) ───────

    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_LimiteStoricoEsatto_PasswordRifiutata()
    {
        var user = MakeUser("CurrentPass123!");
        _mockUserRepo.Setup(r => r.GetByIdAsync("TESTUSER")).ReturnsAsync(user);

        _mockPersonalisationRepo
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Personalisation, bool>>>()))
            .ReturnsAsync(new List<Personalisation>
            {
                new() { ParId = "PasswordHistoryLimit", ParValue = "3" }
            });

        _mockPasswordHistoryRepo
            .Setup(r => r.IsPasswordInHistoryAsync("TESTUSER", "NewPass456!", 3))
            .ReturnsAsync(true);

        var command = new ChangePasswordCommand("testuser", "CurrentPass123!", "NewPass456!", "127.0.0.1");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("HISTORY_VIOLATION", result.ResultCode);

        _mockPasswordHistoryRepo.Verify(r => r.IsPasswordInHistoryAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            3),
            Times.Once);
    }

    // ── S-04: Oltre storico limite → password accettata ─────────────────────

    [Fact]
    [Trait("Category", "Security")]
    public async Task Handle_OltreStoricoLimite_PasswordAccettata()
    {
        var user = MakeUser("CurrentPass123!");
        _mockUserRepo.Setup(r => r.GetByIdAsync("TESTUSER")).ReturnsAsync(user);

        _mockPersonalisationRepo
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Personalisation, bool>>>()))
            .ReturnsAsync(new List<Personalisation>
            {
                new() { ParId = "PasswordHistoryLimit", ParValue = "3" }
            });

        _mockPasswordHistoryRepo
            .Setup(r => r.IsPasswordInHistoryAsync("TESTUSER", "NewPass456!", 3))
            .ReturnsAsync(false);

        var command = new ChangePasswordCommand("testuser", "CurrentPass123!", "NewPass456!", "127.0.0.1");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("OK", result.ResultCode);

        _mockPasswordHistoryRepo.Verify(r => r.IsPasswordInHistoryAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            3),
            Times.Once);
    }
}
