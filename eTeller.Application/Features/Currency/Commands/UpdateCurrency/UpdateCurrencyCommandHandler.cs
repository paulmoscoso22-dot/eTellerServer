using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Currency;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Currency.Commands.UpdateCurrency
{
    public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, CurrencyVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCurrencyCommandHandler> _logger;

        public UpdateCurrencyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateCurrencyCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CurrencyVm> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for CurId={CurId}, CurCutId={CurCutId}",
                nameof(UpdateCurrencyCommand), request.CurId, request.CurCutId);

            var existing = await _unitOfWork.CurrencyRepository.GetByKeyAsync(request.CurId, request.CurCutId);

            if (existing is null)
            {
                _logger.LogWarning("Currency not found for update: {CurId}/{CurCutId}", request.CurId, request.CurCutId);
                throw new InvalidOperationException($"Divisa '{request.CurId}/{request.CurCutId}' non trovata.");
            }

            await _unitOfWork.CurrencyRepository.UpdateCurrencyAsync(
                request.CurId,
                request.CurCutId,
                request.CurMinamn,
                request.CurFinezza,
                request.CurTolrat);

            _logger.LogInformation("Currency {CurId}/{CurCutId} updated successfully by {User}",
                request.CurId, request.CurCutId, request.TraUser);

            var updated = await _unitOfWork.CurrencyRepository.GetByKeyAsync(request.CurId, request.CurCutId);
            return _mapper.Map<CurrencyVm>(updated);
        }
    }
}
