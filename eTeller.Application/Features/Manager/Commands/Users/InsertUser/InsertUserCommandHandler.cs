using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.User;
using eTeller.Application.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Users.InsertUser
{
    public class InsertUserCommandHandler : IRequestHandler<InsertUserCommand, InsertUserVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<InsertUserCommandHandler> _logger;

        public InsertUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<InsertUserCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<InsertUserVm> Handle(InsertUserCommand request, CancellationToken cancellationToken)
        {
            const string defaultPassword = "DefaultPassword";
            var passwordDefaul = await _unitOfWork.Repository<Domain.Models.Personalisation>().GetAsync(p => p.ParId == defaultPassword);
            if(!passwordDefaul.Any())
            {
                _logger.LogError("Default password not found in Personalisation table");
                throw new InvalidOperationException("Default password not found in Personalisation table");
            }
            _logger.LogInformation("Handling {CommandName}", nameof(InsertUserCommand));

            var existingUser = await _unitOfWork.Repository<Domain.Models.User>()
                .GetAsync(u => u.UsrHostId == request.UsrHostId);

            if (existingUser.Any())
            {
                _logger.LogWarning("User with Host ID {UsrHostId} already exists", request.UsrHostId);
                throw new InvalidOperationException($"User with Host ID '{request.UsrHostId}' already exists");
            }

            var newUser = new Domain.Models.User
            {
                UsrId = request.UsrId,
                UsrHostId = request.UsrHostId,
                UsrBraId = request.UsrBraId,
                UsrStatus = request.UsrStatus,
                UsrExtref = request.UsrExtref,
                UsrPass = Utility.CifraPass(passwordDefaul[0].ParValue),
                UsrChgPas = true,
                UsrLingua = request.UsrLingua
            };

            _unitOfWork.Repository<Domain.Models.User>().AddEntity(newUser);
            var userSaved = await _unitOfWork.Complete();

            if (userSaved == 0)
            {
                _logger.LogError("Failed to insert user {UsrId}", request.UsrId);
                throw new InvalidOperationException($"Failed to insert user '{request.UsrId}'");
            }

            var insertedUser = (await _unitOfWork.Repository<Domain.Models.User>()
                .GetAsync(u => u.UsrId == request.UsrId)).FirstOrDefault();

            if (insertedUser == null)
            {
                _logger.LogError("Failed to retrieve inserted user {UsrId}", request.UsrId);
                throw new InvalidOperationException($"Failed to retrieve inserted user");
            }

            newUser = insertedUser;

            var traceSaved = await _unitOfWork.TraceRepository.InsertTrace(
                traTime: DateTime.Now,
                traUser: request.TraUser,
                traFunCode: "OPE",
                traSubFun: "INSERTUSER",
                traStation: request.TraStation,
                traTabNam: "sys_USERS",
                traEntCode: newUser.UsrId,
                traRevTrxTrace: null,
                traDes: $"INSERT USER: ID: {newUser.UsrId} HOST_ID: {newUser.UsrHostId} BRA_ID: {newUser.UsrBraId} STATUS: {newUser.UsrStatus} LINGUA: {newUser.UsrLingua}",
                traExtRef: null,
                traError: false
            );

            if (traceSaved == 0)
            {
                _logger.LogError("Failed to insert trace for user {UsrId}", request.UsrId);
                throw new InvalidOperationException($"Failed to insert trace for user '{request.UsrId}'");
            }

            _logger.LogInformation("Handled {CommandName}, created user {UsrId}", nameof(InsertUserCommand), request.UsrId);

            return new InsertUserVm
            {
                UsrId = request.UsrId,
                UsrHostId = request.UsrHostId,
                UsrBraId = request.UsrBraId,
                UsrStatus = request.UsrStatus,
                UsrExtref = request.UsrExtref,
                UsrLingua = request.UsrLingua
            };
        }
     }
}