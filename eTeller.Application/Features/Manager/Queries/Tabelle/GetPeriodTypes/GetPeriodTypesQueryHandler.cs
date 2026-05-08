using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager.Tabelle;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetPeriodTypes
{
    public class GetPeriodTypesQueryHandler : IRequestHandler<GetPeriodTypesQuery, IEnumerable<PeriodTypeVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPeriodTypesQueryHandler> _logger;

        public GetPeriodTypesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetPeriodTypesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<PeriodTypeVm>> Handle(GetPeriodTypesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetPeriodTypesQuery));

            var entities = await _unitOfWork.Repository<ST_PERIODICITA>().GetAllAsync();
            var result = _mapper.Map<IEnumerable<PeriodTypeVm>>(entities);

            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetPeriodTypesQuery), result?.Count() ?? 0);
            return result;
        }
    }
}
