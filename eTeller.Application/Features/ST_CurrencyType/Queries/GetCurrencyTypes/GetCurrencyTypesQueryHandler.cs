using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.ST_CurrencyType;
using CutModel = eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.ST_CurrencyType.Queries.GetCurrencyTypes
{
    public class GetCurrencyTypesQueryHandler : IRequestHandler<GetCurrencyTypesQuery, IEnumerable<ST_CurrencyTypeVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCurrencyTypesQueryHandler> _logger;

        public GetCurrencyTypesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCurrencyTypesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ST_CurrencyTypeVm>> Handle(GetCurrencyTypesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetCurrencyTypesQuery));
            
            var currencyTypeRepository = _unitOfWork.Repository<CutModel.ST_CurrencyType>();
            var currencyTypes = await currencyTypeRepository.GetAllAsync();
            
            var currencyTypeVms = _mapper.Map<IEnumerable<ST_CurrencyTypeVm>>(currencyTypes);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetCurrencyTypesQuery), currencyTypeVms?.Count() ?? 0);
            
            return currencyTypeVms;
        }
    }
}
