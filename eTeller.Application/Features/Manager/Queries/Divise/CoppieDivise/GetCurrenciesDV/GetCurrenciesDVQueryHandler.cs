using eTeller.Application.Contracts;
using eTeller.Application.Mappings.CurrencyCouple;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Divise.CoppieDivise.GetCurrenciesDV
{
    public class GetCurrenciesDVQueryHandler : IRequestHandler<GetCurrenciesDVQuery, IEnumerable<CurrencyDvVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCurrenciesDVQueryHandler> _logger;

        public GetCurrenciesDVQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCurrenciesDVQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<CurrencyDvVm>> Handle(GetCurrenciesDVQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetCurrenciesDVQuery));
            return await _unitOfWork.CurrencyCoupleRepository.GetCurrenciesDVAsync();
        }
    }
}
