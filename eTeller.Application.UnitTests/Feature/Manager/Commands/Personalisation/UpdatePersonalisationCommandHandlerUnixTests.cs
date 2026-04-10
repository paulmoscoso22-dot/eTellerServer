using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Contracts.Personalisation;
using eTeller.Application.Contracts.Trace;
using eTeller.Application.Features.Manager.Commands.Pesonalisation.UpdatePersonalisation;
using eTeller.Application.Mappings.Personalisation;
using Microsoft.Extensions.Logging;
using Moq;

namespace eTeller.Application.UnitTests.Feature.Manager.Commands.Personalisation
{
    public class UpdatePersonalisationCommandHandlerXUnitTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UpdatePersonalisationCommandHandler>> _mockLogger;
        private readonly Mock<IPersonalisationRepository> _mockPersonalisationRepo;
        private readonly Mock<ITraceRepository> _mockTraceRepo;
        private readonly UpdatePersonalisationCommandHandler _handler;

        public UpdatePersonalisationCommandHandlerXUnitTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UpdatePersonalisationCommandHandler>>();
            _mockPersonalisationRepo = new Mock<IPersonalisationRepository>();
            _mockTraceRepo = new Mock<ITraceRepository>();

            _mockUnitOfWork.Setup(x => x.PersonalisationRepository).Returns(_mockPersonalisationRepo.Object);
            _mockUnitOfWork.Setup(x => x.TraceRepository).Returns(_mockTraceRepo.Object);
            _mockUnitOfWork.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.Complete()).ReturnsAsync(1);
            _mockUnitOfWork.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.Rollback()).Returns(Task.CompletedTask);

            _handler = new UpdatePersonalisationCommandHandler(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ReturnsPersonalisationVm_WhenUpdateSucceeds()
        {
            var originalParId = "ORIGINAL_ID";
            var newParId = "NEW_ID";
            var parDes = "Description";
            var parValue = "Value";
            var command = new UpdatePersonalisationCommand(newParId, parDes, parValue, originalParId);

            var personalisation = new Domain.Models.Personalisation
            {
                ParId = newParId,
                ParDes = parDes,
                ParValue = parValue
            };

            var expectedVm = new PersonalisationVm
            {
                ParId = newParId,
                ParDes = parDes,
                ParValue = parValue
            };

            _mockPersonalisationRepo
                .Setup(x => x.PersonalisationUpdateAsync(newParId, parDes, parValue, originalParId))
                .ReturnsAsync(personalisation);

            _mockTraceRepo
                .Setup(x => x.InsertTrace(
                    It.IsAny<DateTime>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(1);

            _mockMapper
                .Setup(m => m.Map<PersonalisationVm>(personalisation))
                .Returns(expectedVm);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(newParId, result.ParId);
            Assert.Equal(parDes, result.ParDes);
            Assert.Equal(parValue, result.ParValue);

            _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _mockPersonalisationRepo.Verify(x => x.PersonalisationUpdateAsync(newParId, parDes, parValue, originalParId), Times.Once);
            _mockUnitOfWork.Verify(x => x.Complete(), Times.Once);
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            _mockTraceRepo.Verify(x => x.InsertTrace(
                It.IsAny<DateTime>(),
                "SYSTEM",
                "UPD",
                null,
                "SERVER",
                "PERSONALISATION",
                newParId,
                null,
                It.IsAny<string?>(),
                null,
                false), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsKeyNotFoundException_WhenPersonalisationNotFound()
        {
            var originalParId = "MISSING_ID";
            var newParId = "NEW_ID";
            var parDes = "Description";
            var parValue = "Value";
            var command = new UpdatePersonalisationCommand(newParId, parDes, parValue, originalParId);

            _mockPersonalisationRepo
                .Setup(x => x.PersonalisationUpdateAsync(newParId, parDes, parValue, originalParId))
                .ReturnsAsync((Domain.Models.Personalisation?)null);

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Contains(originalParId, exception.Message);
            Assert.NotNull(exception.InnerException);
            Assert.IsType<KeyNotFoundException>(exception.InnerException);

            _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(x => x.Rollback(), Times.Once);
            _mockUnitOfWork.Verify(x => x.Complete(), Times.Never);
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ThrowsApplicationException_WhenRepositoryThrows()
        {
            var originalParId = "ERROR_ID";
            var newParId = "NEW_ID";
            var parDes = "Description";
            var parValue = "Value";
            var command = new UpdatePersonalisationCommand(newParId, parDes, parValue, originalParId);

            _mockPersonalisationRepo
                .Setup(x => x.PersonalisationUpdateAsync(newParId, parDes, parValue, originalParId))
                .ThrowsAsync(new Exception("Database error"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Contains("Database error", exception.Message);
            Assert.NotNull(exception.InnerException);

            _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(x => x.Rollback(), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsApplicationException_WhenTraceInsertFails()
        {
            var originalParId = "ORIGINAL_ID";
            var newParId = "NEW_ID";
            var parDes = "Description";
            var parValue = "Value";
            var command = new UpdatePersonalisationCommand(newParId, parDes, parValue, originalParId);

            var personalisation = new Domain.Models.Personalisation
            {
                ParId = newParId,
                ParDes = parDes,
                ParValue = parValue
            };

            _mockPersonalisationRepo
                .Setup(x => x.PersonalisationUpdateAsync(newParId, parDes, parValue, originalParId))
                .ReturnsAsync(personalisation);

            _mockTraceRepo
                .Setup(x => x.InsertTrace(
                    It.IsAny<DateTime>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>()))
                .ThrowsAsync(new Exception("Trace error"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Contains("Trace error", exception.Message);

            _mockUnitOfWork.Verify(x => x.Rollback(), Times.Once);
        }

        [Fact]
        public async Task Handle_VerifyTraceContent_WhenUpdateSucceeds()
        {
            var originalParId = "ORIGINAL_ID";
            var newParId = "NEW_ID";
            var parDes = "Description";
            var parValue = "Value";
            var command = new UpdatePersonalisationCommand(newParId, parDes, parValue, originalParId);

            var personalisation = new Domain.Models.Personalisation
            {
                ParId = newParId,
                ParDes = parDes,
                ParValue = parValue
            };

            var expectedVm = new PersonalisationVm
            {
                ParId = newParId,
                ParDes = parDes,
                ParValue = parValue
            };

            _mockPersonalisationRepo
                .Setup(x => x.PersonalisationUpdateAsync(newParId, parDes, parValue, originalParId))
                .ReturnsAsync(personalisation);

            string? capturedDescription = null;
            _mockTraceRepo
                .Setup(x => x.InsertTrace(
                    It.IsAny<DateTime>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>()))
                .Callback<DateTime, string, string, string?, string, string, string, string?, string?, string?, bool>(
                    (traTime, traUser, traFunCode, traSubFun, traStation, traTabNam, traEntCode, traRevTrxTrace, traDes, traExtRef, traError) =>
                    {
                        capturedDescription = traDes;
                    })
                .ReturnsAsync(1);

            _mockMapper
                .Setup(m => m.Map<PersonalisationVm>(personalisation))
                .Returns(expectedVm);

            await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(capturedDescription);
            Assert.Contains(originalParId, capturedDescription);
            Assert.Contains(newParId, capturedDescription);
        }

        [Fact]
        public async Task Handle_RollbackCalled_WhenCompleteThrows()
        {
            var originalParId = "ORIGINAL_ID";
            var newParId = "NEW_ID";
            var parDes = "Description";
            var parValue = "Value";
            var command = new UpdatePersonalisationCommand(newParId, parDes, parValue, originalParId);

            var personalisation = new Domain.Models.Personalisation
            {
                ParId = newParId,
                ParDes = parDes,
                ParValue = parValue
            };

            _mockPersonalisationRepo
                .Setup(x => x.PersonalisationUpdateAsync(newParId, parDes, parValue, originalParId))
                .ReturnsAsync(personalisation);

            _mockTraceRepo
                .Setup(x => x.InsertTrace(
                    It.IsAny<DateTime>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(1);

            _mockUnitOfWork.Setup(x => x.Complete()).ThrowsAsync(new Exception("Complete failed"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));

            _mockUnitOfWork.Verify(x => x.Rollback(), Times.Once);
        }

        [Fact]
        public async Task Handle_VerifyAllParameters_MapCorrectly()
        {
            var originalParId = "TEST_ORIG";
            var newParId = "TEST_NEW";
            var parDes = "Test Description";
            var parValue = "Test Value";
            var command = new UpdatePersonalisationCommand(newParId, parDes, parValue, originalParId);

            var personalisation = new Domain.Models.Personalisation
            {
                ParId = newParId,
                ParDes = parDes,
                ParValue = parValue
            };

            var expectedVm = new PersonalisationVm
            {
                ParId = newParId,
                ParDes = parDes,
                ParValue = parValue
            };

            _mockPersonalisationRepo
                .Setup(x => x.PersonalisationUpdateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(personalisation)
                .Callback<string, string, string, string>((parId, parDesParam, parVal, origParId) =>
                {
                    Assert.Equal(newParId, parId);
                    Assert.Equal(parDes, parDesParam);
                    Assert.Equal(parValue, parVal);
                    Assert.Equal(originalParId, origParId);
                });

            _mockTraceRepo
                .Setup(x => x.InsertTrace(
                    It.IsAny<DateTime>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(1);

            _mockMapper
                .Setup(m => m.Map<PersonalisationVm>(It.IsAny<Domain.Models.Personalisation>()))
                .Returns(expectedVm);

            await _handler.Handle(command, CancellationToken.None);

            _mockPersonalisationRepo.Verify(
                x => x.PersonalisationUpdateAsync(newParId, parDes, parValue, originalParId),
                Times.Once);
        }
    }
}