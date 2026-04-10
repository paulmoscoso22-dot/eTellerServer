using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Vigilanza;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetSpAntirecAppearerByParameters
{
    public class GetSpAntirecAppearerByParametersQueryHandler : IRequestHandler<GetSpAntirecAppearerByParametersQuery, IEnumerable<AppearerAllVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSpAntirecAppearerByParametersQueryHandler> _logger;

        public GetSpAntirecAppearerByParametersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSpAntirecAppearerByParametersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AppearerAllVm>> Handle(GetSpAntirecAppearerByParametersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with parameters Nome1={Nome1}, Nome2={Nome2}, Nome3={Nome3}, Nome4={Nome4}, AraBirthdate={AraBirthdate}, AraRecComplete={AraRecComplete}, MinRecdate={MinRecdate}",
                nameof(GetSpAntirecAppearerByParametersQuery), request.Nome1, request.Nome2, request.Nome3, request.Nome4, request.AraBirthdate, request.AraRecComplete, request.MinRecdate);

            var appearers = await _unitOfWork.VigilanzaRepository.GetAppearerByParameters(
                request.Nome1,
                request.Nome2,
                request.Nome3,
                request.Nome4,
                request.AraBirthdate,
                request.AraRecComplete,
                request.MinRecdate);

            var appearersVms = _mapper.Map<IEnumerable<AppearerAllVm>>(appearers);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetSpAntirecAppearerByParametersQuery), appearersVms?.Count() ?? 0);
            return appearersVms ?? Enumerable.Empty<AppearerAllVm>();
        }
    }
}
