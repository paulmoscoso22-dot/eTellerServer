using eTeller.Application.Contracts;
using eTeller.Application.Contracts.Manager;
using eTeller.Application.Features.Manager.Commands.Functions.InsertSysFunction;
using Microsoft.Extensions.Logging;
using Moq;

namespace eTeller.Application.UnitTests.Feature.Manager.Commands.Functions
{
    public class InsertSysFunctionCommandHandlerXUnitTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IManagerRepository> _mockManagerRepo;
        private readonly Mock<ILogger<InsertSysFunctionCommandHandler>> _mockLogger;
        private readonly InsertSysFunctionCommandHandler _handler;

        public InsertSysFunctionCommandHandlerXUnitTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockManagerRepo = new Mock<IManagerRepository>();
            _mockLogger = new Mock<ILogger<InsertSysFunctionCommandHandler>>();

            _mockUnitOfWork.Setup(x => x.ManagerRepository).Returns(_mockManagerRepo.Object);

            _handler = new InsertSysFunctionCommandHandler(
                _mockUnitOfWork.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ReturnsTrue_WhenInsertSucceeds()
        {
            var command = new InsertSysFunctionCommand(
                "ADMIN_USER",
                "SERVER_01",
                "TEST_FUNCTION",
                "Test function description",
                100,
                false);

            _mockManagerRepo
                .Setup(x => x.InsertSysFunctionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            _mockManagerRepo.Verify(
                x => x.InsertSysFunctionAsync(
                    "ADMIN_USER",
                    "SERVER_01",
                    "TEST_FUNCTION",
                    "Test function description",
                    100,
                    false,
                    CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenInsertFails()
        {
            var command = new InsertSysFunctionCommand(
                "ADMIN_USER",
                "SERVER_01",
                "TEST_FUNCTION",
                "Test function description",
                100,
                false);

            _mockManagerRepo
                .Setup(x => x.InsertSysFunctionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            var command = new InsertSysFunctionCommand(
                "ADMIN_USER",
                "SERVER_01",
                "TEST_FUNCTION",
                "Test function description",
                100,
                false);

            _mockManagerRepo
                .Setup(x => x.InsertSysFunctionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_VerifyCorrectParameters_WhenInsertSucceeds()
        {
            var traUser = "ADMIN_USER";
            var traStation = "SERVER_01";
            var funName = "NEW_FUNCTION";
            var funDescription = "Description";
            var funHostcode = 200;
            var offline = true;

            var command = new InsertSysFunctionCommand(
                traUser,
                traStation,
                funName,
                funDescription,
                funHostcode,
                offline);

            string? capturedTraUser = null;
            string? capturedTraStation = null;
            string? capturedFunName = null;
            string? capturedFunDescription = null;
            int capturedFunHostcode = 0;
            bool capturedOffline = false;

            _mockManagerRepo
                .Setup(x => x.InsertSysFunctionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, string, string, string?, int, bool, CancellationToken>(
                    (user, station, name, desc, hostcode, off, ct) =>
                    {
                        capturedTraUser = user;
                        capturedTraStation = station;
                        capturedFunName = name;
                        capturedFunDescription = desc;
                        capturedFunHostcode = hostcode;
                        capturedOffline = off;
                    })
                .ReturnsAsync(true);

            await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(traUser, capturedTraUser);
            Assert.Equal(traStation, capturedTraStation);
            Assert.Equal(funName, capturedFunName);
            Assert.Equal(funDescription, capturedFunDescription);
            Assert.Equal(funHostcode, capturedFunHostcode);
            Assert.Equal(offline, capturedOffline);
        }

        [Fact]
        public async Task Handle_PassesCancellationToken_ToRepository()
        {
            var command = new InsertSysFunctionCommand(
                "ADMIN_USER",
                "SERVER_01",
                "TEST_FUNCTION",
                "Test description",
                100,
                false);

            using var cts = new CancellationTokenSource();

            _mockManagerRepo
                .Setup(x => x.InsertSysFunctionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _handler.Handle(command, cts.Token);

            _mockManagerRepo.Verify(
                x => x.InsertSysFunctionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    cts.Token),
                Times.Once);
        }

        [Fact]
        public async Task Handle_VerifyLoggerCalled_WhenInsertFails()
        {
            var command = new InsertSysFunctionCommand(
                "ADMIN_USER",
                "SERVER_01",
                "FAILED_FUNCTION",
                "Test description",
                100,
                false);

            _mockManagerRepo
                .Setup(x => x.InsertSysFunctionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithNullDescription_PassesNullToRepository()
        {
            var command = new InsertSysFunctionCommand(
                "ADMIN_USER",
                "SERVER_01",
                "TEST_FUNCTION",
                null,
                100,
                false);

            string? capturedDesc = "initial";

            _mockManagerRepo
                .Setup(x => x.InsertSysFunctionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, string, string, string?, int, bool, CancellationToken>(
                    (user, station, name, desc, hostcode, off, ct) => { capturedDesc = desc; })
                .ReturnsAsync(true);

            await _handler.Handle(command, CancellationToken.None);

            Assert.Null(capturedDesc);
        }

        [Fact]
        public async Task Handle_WithTrueOffline_PassesTrueToRepository()
        {
            var command = new InsertSysFunctionCommand(
                "ADMIN_USER",
                "SERVER_01",
                "TEST_FUNCTION",
                "Description",
                100,
                true);

            bool capturedOffline = false;

            _mockManagerRepo
                .Setup(x => x.InsertSysFunctionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, string, string, string?, int, bool, CancellationToken>(
                    (user, station, name, desc, hostcode, off, ct) => { capturedOffline = off; })
                .ReturnsAsync(true);

            await _handler.Handle(command, CancellationToken.None);

            Assert.True(capturedOffline);
        }

        [Fact]
        public async Task Handle_WithFalseOffline_PassesFalseToRepository()
        {
            var command = new InsertSysFunctionCommand(
                "ADMIN_USER",
                "SERVER_01",
                "TEST_FUNCTION",
                "Description",
                100,
                false);

            bool capturedOffline = true;

            _mockManagerRepo
                .Setup(x => x.InsertSysFunctionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, string, string, string?, int, bool, CancellationToken>(
                    (user, station, name, desc, hostcode, off, ct) => { capturedOffline = off; })
                .ReturnsAsync(true);

            await _handler.Handle(command, CancellationToken.None);

            Assert.False(capturedOffline);
        }
    }
}