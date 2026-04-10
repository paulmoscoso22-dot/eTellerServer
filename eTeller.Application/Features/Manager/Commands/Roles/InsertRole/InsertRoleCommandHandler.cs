using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Roles.InsertRole
{
    public class InsertRoleCommandHandler : IRequestHandler<InsertRoleCommand, SysRoleVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<InsertRoleCommandHandler> _logger;

        public InsertRoleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<InsertRoleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SysRoleVm> Handle(InsertRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                _logger.LogInformation("Handling {CommandName}", nameof(InsertRoleCommand));

                var existingRole = await _unitOfWork.Repository<Domain.Models.sys_ROLE>()
                    .GetAsync(r => r.RoleName == request.RoleName);

                if (existingRole.Any())
                {
                    _logger.LogWarning("Role with name {RoleName} already exists", request.RoleName);
                    throw new InvalidOperationException($"Role with name '{request.RoleName}' already exists");
                }

                var newRole = new Domain.Models.sys_ROLE
                {
                    RoleName = request.RoleName,
                    RoleDes = request.RoleDes
                };

                _unitOfWork.Repository<Domain.Models.sys_ROLE>().AddEntity(newRole);
                await _unitOfWork.Complete();

                var insertedRole = (await _unitOfWork.Repository<Domain.Models.sys_ROLE>()
                    .GetAsync(r => r.RoleName == request.RoleName)).FirstOrDefault();

                if (insertedRole == null)
                {
                    _logger.LogError("Failed to retrieve inserted role {RoleName}", request.RoleName);
                    throw new InvalidOperationException($"Failed to retrieve inserted role");
                }

                newRole = insertedRole;

                await _unitOfWork.TraceRepository.InsertTrace(
                    traTime: DateTime.Now,
                    traUser: request.traUser,
                    traFunCode: "OPE",
                    traSubFun: "INSERTROLE",
                    traStation: request.traStation,
                    traTabNam: "sys_ROLE",
                    traEntCode: newRole.RoleId.ToString(),
                    traRevTrxTrace: null,
                    traDes: $"INSERT ROLE: ID: {newRole.RoleId} NAME: {newRole.RoleName} DES: {newRole.RoleDes}",
                    traExtRef: null,
                    traError: false
                );
                await _unitOfWork.Complete();
                await _unitOfWork.CommitAsync(); 

                _logger.LogInformation("Handled {CommandName}, created role {RoleName}", nameof(InsertRoleCommand), request.RoleName);

                return _mapper.Map<SysRoleVm>(newRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {CommandName}", nameof(InsertRoleCommand));
                await _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
