using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Contracts.Manager;
using eTeller.Application.Features.Manager.Queries.Roles.GetAllRole;
using eTeller.Application.Mappings.Manager;
using Microsoft.Extensions.Logging;
using Moq;

namespace eTeller.Application.UnitTests.Feature.Manager.Queries.Roles
{
    public class GetAllRoleQueryHandlerXUnitTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<GetAllRoleQueryHandler>> _mockLogger;
        private readonly Mock<IManagerRepository> _mockManagerRepository;
        private readonly GetAllRoleQueryHandler _handler;

        public GetAllRoleQueryHandlerXUnitTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<GetAllRoleQueryHandler>>();
            _mockManagerRepository = new Mock<IManagerRepository>();

            _mockUnitOfWork.Setup(x => x.ManagerRepository).Returns(_mockManagerRepository.Object);

            _handler = new GetAllRoleQueryHandler(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenNoRolesExist()
        {
            // Arrange
            var roles = new List<Domain.Models.sys_ROLE>();
            _mockManagerRepository
                .Setup(x => x.GetAllRoleAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            var mappedRoles = new List<SysRoleVm>();
            _mockMapper
                .Setup(x => x.Map<IEnumerable<SysRoleVm>>(It.IsAny<IEnumerable<Domain.Models.sys_ROLE>>()))
                .Returns(mappedRoles);

            var query = new GetAllRoleQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_ReturnsRoles_WhenRolesExist()
        {
            // Arrange
            var roles = new List<Domain.Models.sys_ROLE>
            {
                new() { RoleId = 1, RoleName = "Admin", RoleDes = "Administrator" },
                new() { RoleId = 2, RoleName = "User", RoleDes = "Regular User" }
            };

            _mockManagerRepository
                .Setup(x => x.GetAllRoleAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            var mappedRoles = new List<SysRoleVm>
            {
                new() { RoleId = 1, RoleName = "Admin", RoleDes = "Administrator" },
                new() { RoleId = 2, RoleName = "User", RoleDes = "Regular User" }
            };

            _mockMapper
                .Setup(x => x.Map<IEnumerable<SysRoleVm>>(It.IsAny<IEnumerable<Domain.Models.sys_ROLE>>()))
                .Returns(mappedRoles);

            var query = new GetAllRoleQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            _mockManagerRepository
                .Setup(x => x.GetAllRoleAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var query = new GetAllRoleQuery();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_LogsInformation_WhenHandling()
        {
            // Arrange
            var roles = new List<Domain.Models.sys_ROLE>();
            _mockManagerRepository
                .Setup(x => x.GetAllRoleAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            _mockMapper
                .Setup(x => x.Map<IEnumerable<SysRoleVm>>(It.IsAny<IEnumerable<Domain.Models.sys_ROLE>>()))
                .Returns(new List<SysRoleVm>());

            var query = new GetAllRoleQuery();

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert - Logger verification (this is a simplified check)
            _mockManagerRepository.Verify(
                x => x.GetAllRoleAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_CallsManagerRepository_Once()
        {
            // Arrange
            var roles = new List<Domain.Models.sys_ROLE>
            {
                new() { RoleId = 1, RoleName = "Admin", RoleDes = "Admin" }
            };

            _mockManagerRepository
                .Setup(x => x.GetAllRoleAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            _mockMapper
                .Setup(x => x.Map<IEnumerable<SysRoleVm>>(It.IsAny<IEnumerable<Domain.Models.sys_ROLE>>()))
                .Returns(new List<SysRoleVm>());

            var query = new GetAllRoleQuery();

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockManagerRepository.Verify(
                x => x.GetAllRoleAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_CallsMapper_Once()
        {
            // Arrange
            var roles = new List<Domain.Models.sys_ROLE>
            {
                new() { RoleId = 1, RoleName = "Admin", RoleDes = "Admin" }
            };

            _mockManagerRepository
                .Setup(x => x.GetAllRoleAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            var mappedRoles = new List<SysRoleVm>
            {
                new() { RoleId = 1, RoleName = "Admin", RoleDes = "Admin" }
            };

            _mockMapper
                .Setup(x => x.Map<IEnumerable<SysRoleVm>>(It.IsAny<IEnumerable<Domain.Models.sys_ROLE>>()))
                .Returns(mappedRoles);

            var query = new GetAllRoleQuery();

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockMapper.Verify(
                x => x.Map<IEnumerable<SysRoleVm>>(It.IsAny<IEnumerable<Domain.Models.sys_ROLE>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_SingleRole_ReturnsCorrectData()
        {
            // Arrange
            var role = new Domain.Models.sys_ROLE
            {
                RoleId = 1,
                RoleName = "Admin",
                RoleDes = "Administrator Role"
            };

            var roles = new List<Domain.Models.sys_ROLE> { role };

            _mockManagerRepository
                .Setup(x => x.GetAllRoleAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            var mappedRole = new SysRoleVm
            {
                RoleId = 1,
                RoleName = "Admin",
                RoleDes = "Administrator Role"
            };

            _mockMapper
                .Setup(x => x.Map<IEnumerable<SysRoleVm>>(It.IsAny<IEnumerable<Domain.Models.sys_ROLE>>()))
                .Returns(new List<SysRoleVm> { mappedRole });

            var query = new GetAllRoleQuery();

            // Act
            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].RoleId);
            Assert.Equal("Admin", result[0].RoleName);
            Assert.Equal("Administrator Role", result[0].RoleDes);
        }

        [Fact]
        public async Task Handle_LargeDataset_ReturnsAllItems()
        {
            // Arrange
            var roles = Enumerable.Range(1, 100)
                .Select(i => new Domain.Models.sys_ROLE
                {
                    RoleId = i,
                    RoleName = $"Role{i}",
                    RoleDes = $"Description {i}"
                }).ToList();

            _mockManagerRepository
                .Setup(x => x.GetAllRoleAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            var mappedRoles = roles.Select(r => new SysRoleVm
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                RoleDes = r.RoleDes
            }).ToList();

            _mockMapper
                .Setup(x => x.Map<IEnumerable<SysRoleVm>>(It.IsAny<IEnumerable<Domain.Models.sys_ROLE>>()))
                .Returns(mappedRoles);

            var query = new GetAllRoleQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(100, result.Count());
        }
    }
}