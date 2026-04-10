using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Application.Mappings.StatoEntita;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StatoEntita.GetStatoEntita
{
    public class GetStatoEntitaQueryHandler : IRequestHandler<GetStatoEntitaQuery, IEnumerable<STStatoEntitaVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetStatoEntitaQueryHandler> _logger;

        public GetStatoEntitaQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetStatoEntitaQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<STStatoEntitaVm>> Handle(GetStatoEntitaQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for SteId: {SteId}", nameof(GetStatoEntitaQuery), request.SteId);

            var repo = _unitOfWork.Repository<eTeller.Domain.Models.ST_STATOENTITA>();
            var results = await repo.GetAsync(s => s.SteId == request.SteId);

            if (results == null || !results.Any())
            {
                _logger.LogWarning("No StatoEntita found for SteId: {SteId}", request.SteId);
                throw new NotFoundException(nameof(GetStatoEntitaQuery), request.SteId);
            }

            var vms = _mapper.Map<IEnumerable<STStatoEntitaVm>>(results);
            _logger.LogInformation("Retrieved {Count} StatoEntita for SteId: {SteId}", vms.Count(), request.SteId);
            return vms;
        }
    }
}
