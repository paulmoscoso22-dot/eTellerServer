using AutoMapper;
using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Roles.DeleteRole
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteRoleCommandHandler> _logger;

        public DeleteRoleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DeleteRoleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                _logger.LogInformation("Handling {CommandName}", nameof(DeleteRoleCommand));

                var existingRoles = await _unitOfWork.Repository<Domain.Models.sys_ROLE>()
                    .GetAsync(r => r.RoleId == request.RoleId);

                var existingRole = existingRoles.FirstOrDefault();
                if (existingRole == null)
                {
                    _logger.LogWarning("Role with id {RoleId} not found", request.RoleId);
                    throw new InvalidOperationException($"Role with id '{request.RoleId}' not found");
                }

                var roleName = existingRole.RoleName;
                var roleDes = existingRole.RoleDes;

                _unitOfWork.Repository<Domain.Models.sys_ROLE>().DeleteEntity(existingRole);
                await _unitOfWork.Complete();

                await _unitOfWork.TraceRepository.InsertTrace(
                    traTime: DateTime.Now,
                    traUser: "SYSTEM",
                    traFunCode: "OPE",
                    traSubFun: "DELETEROLE",
                    traStation: "SERVER",
                    traTabNam: "sys_ROLE",
                    traEntCode: request.RoleId.ToString(),
                    traRevTrxTrace: null,
                    traDes: $"DELETE ROLE: ID: {request.RoleId} NAME: {roleName} DES: {roleDes}",
                    traExtRef: null,
                    traError: false
                );

                await _unitOfWork.Complete();
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Handled {CommandName}, deleted role {RoleId}", nameof(DeleteRoleCommand), request.RoleId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {CommandName}, initiating rollback", nameof(DeleteRoleCommand));
                await _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
