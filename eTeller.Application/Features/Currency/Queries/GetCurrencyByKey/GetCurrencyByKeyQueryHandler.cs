using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Currency;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Currency.Queries.GetCurrencyByKey
{
    public class GetCurrencyByKeyQueryHandler : IRequestHandler<GetCurrencyByKeyQuery, CurrencyVm?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCurrencyByKeyQueryHandler> _logger;

        public GetCurrencyByKeyQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCurrencyByKeyQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CurrencyVm?> Handle(GetCurrencyByKeyQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for CurId={CurId}, CurCutId={CurCutId}",
                nameof(GetCurrencyByKeyQuery), request.CurId, request.CurCutId);

            var currency = await _unitOfWork.CurrencyRepository.GetByKeyAsync(request.CurId, request.CurCutId);

            if (currency is null)
            {
                _logger.LogWarning("Currency not found: {CurId}/{CurCutId}", request.CurId, request.CurCutId);
                return null;
            }

            return _mapper.Map<CurrencyVm>(currency);
        }
    }
}
