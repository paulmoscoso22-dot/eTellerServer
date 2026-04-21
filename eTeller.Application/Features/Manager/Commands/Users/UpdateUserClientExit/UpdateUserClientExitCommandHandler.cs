using AutoMapper;
using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eTeller.Domain.Models;

namespace eTeller.Application.Features.Manager.Commands.Users.UpdateUserClientExit
{
    public class UpdateUserClientExitCommandHandler : IRequestHandler<UpdateUserClientExitCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateUserClientExitCommandHandler> _logger;

        public UpdateUserClientExitCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateUserClientExitCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(UpdateUserClientExitCommand request, CancellationToken cancellationToken)
        {

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                _logger.LogInformation("Handling {CommandName} for UsrCliId: {UsrCliId}", nameof(UpdateUserClientExitCommand), request.UsrCliId);

                var repo = _unitOfWork.Repository<SysUsersUseClient>();
                var all = await repo.GetAllAsync();
                var entity = all.FirstOrDefault(x => x.DataOut == null && x.UsrCliId == request.UsrCliId);

                entity.Forced = true;
                entity.DataOut = System.DateTime.Now;
                entity.Logout = true;

                repo.UpdateEntity(entity);
                var updated = await _unitOfWork.Complete();

                if (updated == 0)
                {
                    await _unitOfWork.TraceRepository.InsertTrace(
                        traTime: DateTime.Now,
                        traUser: "User",
                        traFunCode: "SBLU",
                        traSubFun: "LOGOUT",
                        traStation: "SERVER",
                        traTabNam: "USERSuseCLIENT",
                        traEntCode: entity.UsrId + "_" + entity.CliId,
                        traRevTrxTrace: null,
                        traDes: "logout ERR NOT UPDATED CLI_ID: " + entity.CliId + " USR_ID: " + entity.UsrId + " USR_CLI_ID: " + request.UsrCliId,
                        traExtRef: null,
                        traError: true
                    );
                }
                else
                {
                    await _unitOfWork.TraceRepository.InsertTrace(
                        traTime: DateTime.Now,
                        traUser: "User",
                        traFunCode: "SBLU",
                        traSubFun: "LOGOUT",
                        traStation: "SERVER",
                        traTabNam: "USERSuseCLIENT",
                        traEntCode: entity.UsrId + "_" + entity.CliId,
                        traRevTrxTrace: null,
                        traDes: "logout OK CLI_ID: " + entity.CliId + " USR_ID: " + entity.UsrId + " USR_CLI_ID: " + request.UsrCliId,
                        traExtRef: null,
                        traError: false
                    );
                }
                _logger.LogInformation("Updated sys_USERSuseCLIENT for UsrCliId: {UsrCliId}, Forced: {Forced}, Logout: {Logout}", request.UsrCliId, true, true);
                await _unitOfWork.CommitAsync();
                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating sys_USERSuseCLIENT for UsrCliId: {UsrCliId}", request.UsrCliId);
                await _unitOfWork.Rollback();
                throw;
            }
         }
        
    }
}