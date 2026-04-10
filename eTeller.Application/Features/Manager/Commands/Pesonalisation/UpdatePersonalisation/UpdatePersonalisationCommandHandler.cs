using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Personalisation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Pesonalisation.UpdatePersonalisation
{
    public class UpdatePersonalisationCommandHandler : IRequestHandler<UpdatePersonalisationCommand, PersonalisationVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdatePersonalisationCommandHandler> _logger;

        public UpdatePersonalisationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdatePersonalisationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PersonalisationVm> Handle(UpdatePersonalisationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                _logger.LogInformation("Handling {CommandName}", nameof(UpdatePersonalisationCommand));
                var result = await _unitOfWork.PersonalisationRepository.PersonalisationUpdateAsync(
                    request.ParId,
                    request.ParDes,
                    request.ParValue,
                    request.OriginalParId
                );
                if (result == null)
                {
                    _logger.LogWarning("Personalisation with ParId {ParId} not found for update", request.OriginalParId);
                    throw new KeyNotFoundException($"Personalisation with ParId {request.OriginalParId} not found.");
                }
                await _unitOfWork.TraceRepository.InsertTrace(
                    traTime: DateTime.UtcNow,
                    traUser: "SYSTEM",
                    traFunCode: "UPD",
                    traSubFun: null,
                    traStation: "SERVER",
                    traTabNam: "PERSONALISATION",
                    traEntCode: request.ParId,
                    traRevTrxTrace: null,
                    traDes: $"Updated personalisation: {request.OriginalParId} -> {request.ParId}",
                    traExtRef: null,
                    traError: false
                );
                await _unitOfWork.Complete();
                await _unitOfWork.CommitAsync();

                var personalisationVm = _mapper.Map<PersonalisationVm>(result);
                _logger.LogInformation("Handled {CommandName}", nameof(UpdatePersonalisationCommand));

                return personalisationVm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {CommandName}, initiating rollback", nameof(UpdatePersonalisationCommand));
                await _unitOfWork.Rollback();
                throw new ApplicationException($"An error occurred while updating personalisation: {ex.Message}", ex);
            }
        }
    }
}
