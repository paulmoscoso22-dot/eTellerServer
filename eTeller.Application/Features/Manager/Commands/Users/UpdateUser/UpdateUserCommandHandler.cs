using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Users.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, SysUserByIdVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateUserCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the UpdateUserCommand to update an existing user and their roles.
        /// </summary>
        /// <param name="request">The update user command containing user data and role changes.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The updated user view model.</returns>
        /// <exception cref="InvalidOperationException">Thrown when user not found or HostId is duplicate.</exception>
        public async Task<SysUserByIdVm> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName}", nameof(UpdateUserCommand));
            try
            {
                // Avvia transazione per garantire atomicità delle operazioni
                await _unitOfWork.BeginTransactionAsync();

                // Verifica che l'HostId non sia già assegnato a un altro utente
                var existingHostIdUsers = await _unitOfWork.Repository<Domain.Models.User>()
                    .GetAsync(u => u.UsrHostId == request.UsrHostId && u.UsrId != request.UsrId);

                if (existingHostIdUsers != null && existingHostIdUsers.Any())
                {
                    _logger.LogWarning("Host ID {UsrHostId} is already used by another user", request.UsrHostId);
                    throw new InvalidOperationException($"Host ID '{request.UsrHostId}' is already used by another user");
                }

                // Recupera l'utente esistente dal database
                var existingUsers = await _unitOfWork.Repository<Domain.Models.User>()
                    .GetAsync(u => u.UsrId == request.UsrId);

                var existingUser = existingUsers.FirstOrDefault();
                if (existingUser == null)
                {
                    _logger.LogWarning("User with id {UsrId} not found", request.UsrId);
                    throw new InvalidOperationException($"User with id '{request.UsrId}' not found");
                }

                // Aggiorna i campi dell'utente con i valori dalla request
                existingUser.UsrHostId = request.UsrHostId;
                existingUser.UsrBraId = request.UsrBraId;
                existingUser.UsrStatus = request.UsrStatus;
                existingUser.UsrExtref = request.UsrExtref;
                existingUser.UsrLingua = request.UsrLingua;
                _unitOfWork.Repository<Domain.Models.User>().UpdateEntity(existingUser);

                // Aggiunge nuovi ruoli se specificati nella request
                if (request.addIdRoles != null && request.addIdRoles.Length > 0)
                {
                    var userRoles = request.addIdRoles.Select(roleId => new Domain.Models.UserRole
                    {
                        RoleId = roleId,
                        UserId = request.UsrId
                    }).ToList();
                    _unitOfWork.Repository<Domain.Models.UserRole>().AddRangeEntity(userRoles);
                }

                // Rimuove ruoli esistenti se specificati nella request
                if (request.delIdRoles != null && request.delIdRoles.Length > 0)
                {
                    var userRolesToDelete = await _unitOfWork.Repository<Domain.Models.UserRole>().GetAsync(ur => ur.UserId == request.UsrId && request.delIdRoles.Contains(ur.RoleId));

                    if (userRolesToDelete.Any())
                    {
                        _unitOfWork.Repository<Domain.Models.UserRole>().DeleteRangeEntity(userRolesToDelete);
                    }
                }

                // Registra l'operazione di modifica nella tabella trace
                await _unitOfWork.TraceRepository.InsertTrace(
                    traTime: DateTime.Now,
                    traUser: "SYSTEM",
                    traFunCode: "OPE",
                    traSubFun: "UPDATEUSER",
                    traStation: "SERVER",
                    traTabNam: "sys_USERS",
                    traEntCode: existingUser.UsrId,
                    traRevTrxTrace: null,
                    traDes: $"UPDATE USER: ID: {existingUser.UsrId}",
                    traExtRef: null,
                    traError: false
                );

                // Salva le modifiche e conferma la transazione
                await _unitOfWork.Complete();
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Handled {CommandName}, updated user {UsrId}", nameof(UpdateUserCommand), request.UsrId);
                return _mapper.Map<SysUserByIdVm>(existingUser);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {CommandName} for user {UsrId}", nameof(UpdateUserCommand), request.UsrId);
                await _unitOfWork.Rollback();
                throw new InvalidOperationException($"An error occurred while updating the user: {ex.Message}");
            }
        }
    }
}