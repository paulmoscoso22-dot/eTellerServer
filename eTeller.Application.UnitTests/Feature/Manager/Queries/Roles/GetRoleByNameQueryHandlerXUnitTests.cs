using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Features.Manager.Queries.Roles.GetRoleByName;
using eTeller.Application.Mappings.Manager;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace eTeller.Application.UnitTests.Feature.Manager.Queries.Roles
{
    public class GetRoleByNameQueryHandlerXUnitTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<GetRoleByNameQueryHandler>> _mockLogger;
        private readonly Mock<Contracts.Commons.IBaseSimpleRepository.IBaseSimpleRepository<Domain.Models.sys_ROLE>> _mockRoleRepo;
        private readonly GetRoleByNameQueryHandler _handler;

        public GetRoleByNameQueryHandlerXUnitTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<GetRoleByNameQueryHandler>>();
            _mockRoleRepo = new Mock<eTeller.Application.Contracts.Commons.IBaseSimpleRepository.IBaseSimpleRepository<Domain.Models.sys_ROLE>>();

            _mockUnitOfWork.Setup(x => x.Repository<Domain.Models.sys_ROLE>()).Returns(_mockRoleRepo.Object);

            _handler = new GetRoleByNameQueryHandler(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ReturnsRole_WhenRoleExists()
        {
            // Arrange
            var role = new Domain.Models.sys_ROLE { RoleId = 10, RoleName = "TestRole", RoleDes = "Desc" };
            var roles = new List<Domain.Models.sys_ROLE> { role };

            _mockRoleRepo
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Domain.Models.sys_ROLE, bool>>>() ) )
                .ReturnsAsync(roles);

            var mapped = new SysRoleVm { RoleId = 10, RoleName = "TestRole", RoleDes = "Desc" };
            _mockMapper
                .Setup(m => m.Map<SysRoleVm?>(It.IsAny<Domain.Models.sys_ROLE>()))
                .Returns(mapped);

            var query = new GetRoleByNameQuery("TestRole");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result!.RoleId);
            Assert.Equal("TestRole", result.RoleName);

            _mockRoleRepo.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Domain.Models.sys_ROLE, bool>>>()), Times.Once);
            _mockMapper.Verify(m => m.Map<SysRoleVm?>(It.IsAny<Domain.Models.sys_ROLE>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenRoleNotFound()
        {
            // Arrange
            _mockRoleRepo
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Domain.Models.sys_ROLE, bool>>>() ) )
                .ReturnsAsync(new List<Domain.Models.sys_ROLE>());

            _mockMapper
                .Setup(m => m.Map<SysRoleVm?>(null))
                .Returns((SysRoleVm?)null);

            var query = new GetRoleByNameQuery("MissingRole");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _mockRoleRepo.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Domain.Models.sys_ROLE, bool>>>()), Times.Once);
            _mockMapper.Verify(m => m.Map<SysRoleVm?>(null), Times.Once);
        }

        [Fact]
        public async Task Handle_Throws_WhenRepositoryThrows()
        {
            // Arrange
            _mockRoleRepo
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Domain.Models.sys_ROLE, bool>>>() ) )
                .ThrowsAsync(new Exception("DB error"));

            var query = new GetRoleByNameQuery("Any");

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));

            _mockRoleRepo.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Domain.Models.sys_ROLE, bool>>>()), Times.Once);
        }
    }
}
