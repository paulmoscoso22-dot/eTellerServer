using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Roles.UpdateRole
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, SysRoleVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;

        public UpdateRoleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateRoleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SysRoleVm> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {

                await _unitOfWork.BeginTransactionAsync();
                _logger.LogInformation("Handling {CommandName}", nameof(UpdateRoleCommand));

                var existingRoles = await _unitOfWork.Repository<Domain.Models.sys_ROLE>()
                    .GetAsync(r => r.RoleId == request.RoleId);

                var existingRole = existingRoles.FirstOrDefault();
                if (existingRole == null)
                {
                    _logger.LogWarning("Role with id {RoleId} not found", request.RoleId);
                    throw new InvalidOperationException($"Role with id '{request.RoleId}' not found");
                }

                var duplicateRoles = await _unitOfWork.Repository<Domain.Models.sys_ROLE>()
                    .GetAsync(r => r.RoleName == request.RoleName && r.RoleId != request.RoleId);

                if (duplicateRoles.Any())
                {
                    _logger.LogWarning("Role with name {RoleName} already exists", request.RoleName);
                    throw new InvalidOperationException($"Role with name '{request.RoleName}' already exists");
                }

                var oldRoleName = existingRole.RoleName;
                var oldRoleDes = existingRole.RoleDes;

                existingRole.RoleName = request.RoleName;
                existingRole.RoleDes = request.RoleDes;

                _unitOfWork.Repository<Domain.Models.sys_ROLE>().UpdateEntity(existingRole);
                await _unitOfWork.Complete();

                await _unitOfWork.TraceRepository.InsertTrace(
                    traTime: DateTime.Now,
                    traUser: "SYSTEM",
                    traFunCode: "OPE",
                    traSubFun: "UPDATEROLE",
                    traStation: "SERVER",
                    traTabNam: "sys_ROLE",
                    traEntCode: existingRole.RoleId.ToString(),
                    traRevTrxTrace: null,
                    traDes: $"UPDATE ROLE: ID: {existingRole.RoleId} NAME: {oldRoleName} -> {existingRole.RoleName} DES: {oldRoleDes} -> {existingRole.RoleDes}",
                    traExtRef: null,
                    traError: false
                );

                await _unitOfWork.Complete();
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Handled {CommandName}, updated role {RoleId}", nameof(UpdateRoleCommand), request.RoleId);

                return _mapper.Map<SysRoleVm>(existingRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {CommandName}", nameof(UpdateRoleCommand));
                await _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
