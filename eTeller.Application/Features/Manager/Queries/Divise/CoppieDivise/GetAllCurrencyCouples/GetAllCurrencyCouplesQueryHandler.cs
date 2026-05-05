using eTeller.Application.Contracts;
using eTeller.Application.Mappings.CurrencyCouple;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Divise.CoppieDivise.GetAllCurrencyCouples
{
    public class GetAllCurrencyCouplesQueryHandler : IRequestHandler<GetAllCurrencyCouplesQuery, IEnumerable<CurrencyCoupleVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllCurrencyCouplesQueryHandler> _logger;

        public GetAllCurrencyCouplesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllCurrencyCouplesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<CurrencyCoupleVm>> Handle(GetAllCurrencyCouplesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetAllCurrencyCouplesQuery));
            return await _unitOfWork.CurrencyCoupleRepository.GetAllAsync();
        }
    }
}
