using eTeller.Application.Contracts;
using eTeller.Application.Contracts.Manager;
using eTeller.Application.Features.Manager.Commands.Functions.DeleteSysFunction;
using Microsoft.Extensions.Logging;
using Moq;

namespace eTeller.Application.UnitTests.Feature.Manager.Commands.Functions
{
    public class DeleteSysFunctionCommandHandlerUnitTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IManagerRepository> _mockManagerRepo;
        private readonly Mock<ILogger<DeleteSysFunctionCommandHandler>> _mockLogger;
        private readonly DeleteSysFunctionCommandHandler _handler;

        public DeleteSysFunctionCommandHandlerUnitTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockManagerRepo = new Mock<IManagerRepository>();
            _mockLogger = new Mock<ILogger<DeleteSysFunctionCommandHandler>>();

            _mockUnitOfWork.Setup(x => x.ManagerRepository).Returns(_mockManagerRepo.Object);

            _handler = new DeleteSysFunctionCommandHandler(
                _mockUnitOfWork.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ReturnsTrue_WhenDeleteSucceeds()
        {
            var traUser = "TEST_USER";
            var traStation = "TEST_STATION";
            var funId = 123;
            var command = new DeleteSysFunctionCommand(traUser, traStation, funId);

            _mockManagerRepo
                .Setup(x => x.DeleteSysFunctionAsync(traUser, traStation, funId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            _mockManagerRepo.Verify(
                x => x.DeleteSysFunctionAsync(traUser, traStation, funId, CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenDeleteFails()
        {
            var traUser = "TEST_USER";
            var traStation = "TEST_STATION";
            var funId = 999;
            var command = new DeleteSysFunctionCommand(traUser, traStation, funId);

            _mockManagerRepo
                .Setup(x => x.DeleteSysFunctionAsync(traUser, traStation, funId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _mockManagerRepo.Verify(
                x => x.DeleteSysFunctionAsync(traUser, traStation, funId, CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            var traUser = "TEST_USER";
            var traStation = "TEST_STATION";
            var funId = 123;
            var command = new DeleteSysFunctionCommand(traUser, traStation, funId);

            _mockManagerRepo
                .Setup(x => x.DeleteSysFunctionAsync(traUser, traStation, funId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

            _mockManagerRepo.Verify(
                x => x.DeleteSysFunctionAsync(traUser, traStation, funId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_VerifyCorrectParameters_WhenDeleteSucceeds()
        {
            var traUser = "ADMIN_USER";
            var traStation = "SERVER_01";
            var funId = 42;
            var command = new DeleteSysFunctionCommand(traUser, traStation, funId);

            string? capturedTraUser = null;
            string? capturedTraStation = null;
            int capturedFunId = 0;

            _mockManagerRepo
                .Setup(x => x.DeleteSysFunctionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, string, int, CancellationToken>((user, station, id, ct) =>
                {
                    capturedTraUser = user;
                    capturedTraStation = station;
                    capturedFunId = id;
                })
                .ReturnsAsync(true);

            await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(traUser, capturedTraUser);
            Assert.Equal(traStation, capturedTraStation);
            Assert.Equal(funId, capturedFunId);
        }

        [Fact]
        public async Task Handle_PassesCancellationToken_ToRepository()
        {
            var traUser = "TEST_USER";
            var traStation = "TEST_STATION";
            var funId = 123;
            var command = new DeleteSysFunctionCommand(traUser, traStation, funId);
            using var cts = new CancellationTokenSource();

            _mockManagerRepo
                .Setup(x => x.DeleteSysFunctionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _handler.Handle(command, cts.Token);

            _mockManagerRepo.Verify(
                x => x.DeleteSysFunctionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    cts.Token),
                Times.Once);
        }

        [Fact]
        public async Task Handle_VerifyLoggerCalled_WhenDeleteFails()
        {
            var traUser = "TEST_USER";
            var traStation = "TEST_STATION";
            var funId = 999;
            var command = new DeleteSysFunctionCommand(traUser, traStation, funId);

            _mockManagerRepo
                .Setup(x => x.DeleteSysFunctionAsync(traUser, traStation, funId, It.IsAny<CancellationToken>()))
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
    }
}