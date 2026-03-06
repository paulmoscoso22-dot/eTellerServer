using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Vigilanza;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetAppearerByParametersWithExpiry
{
    public class GetAppearerByParametersWithExpiryQueryHandler : IRequestHandler<GetAppearerByParametersWithExpiryQuery, IEnumerable<AppearerAllVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAppearerByParametersWithExpiryQueryHandler> _logger;

        public GetAppearerByParametersWithExpiryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAppearerByParametersWithExpiryQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AppearerAllVm>> Handle(GetAppearerByParametersWithExpiryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Handling {QueryName} with parameters AraName={AraName}, AraBirthdate={AraBirthdate}, AraRecComplete={AraRecComplete}, ShowExpiredRecords={ShowExpiredRecords}, RecordValidityDays={RecordValidityDays}",
                nameof(GetAppearerByParametersWithExpiryQuery), request.AraName, request.AraBirthdate, request.AraRecComplete, request.ShowExpiredRecords, request.RecordValidityDays);

            var appearers = await _unitOfWork.VigilanzaSpRepository.GetAppearerByParametersWithExpiry(
                request.AraName,
                request.AraBirthdate,
                request.AraRecComplete,
                request.ShowExpiredRecords,
                request.RecordValidityDays);

            var appearersVms = _mapper.Map<IEnumerable<AppearerAllVm>>(appearers);
            
            _logger.LogInformation(
                "Handled {QueryName}, returned {Count} items", 
                nameof(GetAppearerByParametersWithExpiryQuery), appearersVms?.Count() ?? 0);
            
            return appearersVms ?? Enumerable.Empty<AppearerAllVm>();
        }
    }
}
