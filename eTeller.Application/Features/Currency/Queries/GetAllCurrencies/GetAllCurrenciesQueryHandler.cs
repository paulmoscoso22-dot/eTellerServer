using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Currency;
using CurModel = eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Currency.Queries.GetAllCurrencies
{
    public class GetAllCurrenciesQueryHandler : IRequestHandler<GetAllCurrenciesQuery, IEnumerable<CurrencyVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllCurrenciesQueryHandler> _logger;

        public GetAllCurrenciesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllCurrenciesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CurrencyVm>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetAllCurrenciesQuery));
            
            var currencyRepository = _unitOfWork.Repository<CurModel.Currency>();
            var currencies = await currencyRepository.GetAllAsync();
            
            var currencyVms = _mapper.Map<IEnumerable<CurrencyVm>>(currencies);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetAllCurrenciesQuery), currencyVms?.Count() ?? 0);
            
            return currencyVms;
        }
    }
}
