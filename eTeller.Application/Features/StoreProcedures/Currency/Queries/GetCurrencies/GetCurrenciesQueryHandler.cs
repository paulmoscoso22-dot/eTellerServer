using AutoMapper;
using eTeller.Application.Contracts.StoreProcedures;
using eTeller.Application.Mappings.Currency;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Currency.Queries.GetCurrencies
{
    public class GetCurrenciesQueryHandler : IRequestHandler<GetCurrenciesQuery, IEnumerable<CurrencyVm>>
    {
        private readonly ICurrencySpRepository _currencySpRepository;
        private readonly IMapper _mapper;

        public GetCurrenciesQueryHandler(ICurrencySpRepository currencySpRepository, IMapper mapper)
        {
            _currencySpRepository = currencySpRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CurrencyVm>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
        {
            var currencies = await _currencySpRepository.GetAllCurrencies();
            return _mapper.Map<IEnumerable<CurrencyVm>>(currencies);
        }
    }
}
