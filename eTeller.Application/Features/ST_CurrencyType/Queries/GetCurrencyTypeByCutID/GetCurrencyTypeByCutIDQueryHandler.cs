using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.ST_CurrencyType;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.ST_CurrencyType.Queries.GetCurrencyTypeByCutID
{
    public class GetCurrencyTypeByCutIDQueryHandler : IRequestHandler<GetCurrencyTypeByCutIDQuery, ST_CurrencyTypeVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCurrencyTypeByCutIDQueryHandler> _logger;

        public GetCurrencyTypeByCutIDQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCurrencyTypeByCutIDQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ST_CurrencyTypeVm> Handle(GetCurrencyTypeByCutIDQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with CutID={CutID}", nameof(GetCurrencyTypeByCutIDQuery), request.CutID);

            var currencyType = await _unitOfWork.ST_CurrencyTypeSpRepository.GetByCutID(request.CutID);

            var currencyTypeVm = _mapper.Map<ST_CurrencyTypeVm>(currencyType);

            _logger.LogInformation("Handled {QueryName}, found={Found}", nameof(GetCurrencyTypeByCutIDQuery), currencyTypeVm != null);

            return currencyTypeVm;
        }
    }
}
