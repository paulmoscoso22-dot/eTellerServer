using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Features.Manager.Commands.Roles.InsertRole;
using eTeller.Application.Mappings.Manager;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace eTeller.Application.UnitTests.Feature.Manager.Commands.Roles
{
    public class InsertRoleCommandHandlerXUnitTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<InsertRoleCommandHandler>> _mockLogger;
        private readonly Mock<eTeller.Application.Contracts.Trace.ITraceRepository> _mockTraceRepository;
        private readonly Mock<eTeller.Application.Contracts.Commons.IBaseSimpleRepository.IBaseSimpleRepository<Domain.Models.sys_ROLE>> _mockRoleRepo;
        private readonly InsertRoleCommandHandler _handler;

        public InsertRoleCommandHandlerXUnitTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<InsertRoleCommandHandler>>();
            _mockTraceRepository = new Mock<eTeller.Application.Contracts.Trace.ITraceRepository>();
            _mockRoleRepo = new Mock<eTeller.Application.Contracts.Commons.IBaseSimpleRepository.IBaseSimpleRepository<Domain.Models.sys_ROLE>>();

            _mockUnitOfWork.Setup(x => x.Repository<Domain.Models.sys_ROLE>()).Returns(_mockRoleRepo.Object);
            _mockUnitOfWork.Setup(x => x.TraceRepository).Returns(_mockTraceRepository.Object);

            _handler = new InsertRoleCommandHandler(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_InsertsRole_WhenRoleDoesNotExist()
        {
            // Arrange
            var command = new InsertRoleCommand("NewRole", "Description", "traUser", "traStation", "info");

            var existingEmpty = new List<Domain.Models.sys_ROLE>();
            var insertedRole = new Domain.Models.sys_ROLE { RoleId = 42, RoleName = "NewRole", RoleDes = "Description" };

            _mockRoleRepo
                .SetupSequence(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Models.sys_ROLE, bool>>>() ) )
                .ReturnsAsync(existingEmpty)
                .ReturnsAsync(new List<Domain.Models.sys_ROLE> { insertedRole });

            _mockUnitOfWork.Setup(x => x.Complete()).ReturnsAsync(1);

            _mockTraceRepository
                .Setup(x => x.InsertTrace(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>()))
                .ReturnsAsync(1);

            _mockMapper
                .Setup(m => m.Map<SysRoleVm>(It.IsAny<Domain.Models.sys_ROLE>()))
                .Returns((Domain.Models.sys_ROLE src) => new SysRoleVm { RoleId = src.RoleId, RoleName = src.RoleName, RoleDes = src.RoleDes });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(42, result.RoleId);
            Assert.Equal("NewRole", result.RoleName);

            _mockRoleRepo.Verify(r => r.AddEntity(It.IsAny<Domain.Models.sys_ROLE>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.Complete(), Times.AtLeastOnce);
            _mockTraceRepository.Verify(t => t.InsertTrace(It.IsAny<DateTime>(), "traUser", "OPE", "INSERTROLE", "traStation", "sys_ROLE", "42", It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), false), Times.Once);
        }

        [Fact]
        public async Task Handle_Throws_WhenRoleAlreadyExists()
        {
            // Arrange
            var command = new InsertRoleCommand("Existing", "Desc", "u", "s", "i");
            var existing = new List<Domain.Models.sys_ROLE> { new Domain.Models.sys_ROLE { RoleId = 1, RoleName = "Existing", RoleDes = "Desc" } };

            _mockRoleRepo
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Models.sys_ROLE, bool>>>()))
                .ReturnsAsync(existing);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));

            _mockUnitOfWork.Verify(u => u.Rollback(), Times.Once);
        }

        [Fact]
        public async Task Handle_Throws_WhenInsertedRoleNotRetrieved()
        {
            // Arrange
            var command = new InsertRoleCommand("RoleX", "Desc", "u", "s", "i");

            _mockRoleRepo
                .SetupSequence(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Models.sys_ROLE, bool>>>() ) )
                .ReturnsAsync(new List<Domain.Models.sys_ROLE>())
                .ReturnsAsync(new List<Domain.Models.sys_ROLE>());

            _mockUnitOfWork.Setup(x => x.Complete()).ReturnsAsync(1);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));

            _mockUnitOfWork.Verify(u => u.Rollback(), Times.Once);
        }
    }
}
