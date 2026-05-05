using eTeller.Application.Contracts;
using eTeller.Application.Mappings.CurrencyCouple;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Divise.CoppieDivise.GetCurrencyCoupleByKey
{
    public class GetCurrencyCoupleByKeyQueryHandler : IRequestHandler<GetCurrencyCoupleByKeyQuery, CurrencyCoupleVm?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCurrencyCoupleByKeyQueryHandler> _logger;

        public GetCurrencyCoupleByKeyQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCurrencyCoupleByKeyQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CurrencyCoupleVm?> Handle(GetCurrencyCoupleByKeyQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for {Cur1}/{Cur2}", nameof(GetCurrencyCoupleByKeyQuery), request.Cur1, request.Cur2);
            return await _unitOfWork.CurrencyCoupleRepository.GetByKeyAsync(request.Cur1, request.Cur2);
        }
    }
}
