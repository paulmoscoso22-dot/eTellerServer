using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.StCountry;
using CutModel = eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StCountry.Queries.GetAllCountry
{
    public class GetAllCountryQueryHandler : IRequestHandler<GetAllCountryQuery, IEnumerable<StCountryVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllCountryQueryHandler> _logger;

        public GetAllCountryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllCountryQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<StCountryVm>> Handle(GetAllCountryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetAllCountryQuery));
            
            var repository = _unitOfWork.Repository<CutModel.ST_COUNTRY>();
            var results = await repository.GetAllAsync();
            
            var vms = _mapper.Map<IEnumerable<StCountryVm>>(results);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetAllCountryQuery), vms?.Count() ?? 0);
            
            return vms;
        }
    }
}