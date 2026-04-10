using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Application.Mappings.StatoEntita;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StatoEntita.GetAllStatiEntita
{
    public class GetAllStatiEntitaQueryHandler : IRequestHandler<GetAllStatiEntitaQuery, IEnumerable<STStatoEntitaVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllStatiEntitaQueryHandler> _logger;

        public GetAllStatiEntitaQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllStatiEntitaQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<STStatoEntitaVm>> Handle(GetAllStatiEntitaQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetAllStatiEntitaQuery));

            var repo = _unitOfWork.Repository<Domain.Models.ST_STATOENTITA>();
            var results = await repo.GetAllAsync();

            if (results == null || !results.Any())
            {
                _logger.LogWarning("No StatoEntita records found");
                throw new NotFoundException(nameof(GetAllStatiEntitaQuery), "none");
            }

            var vms = _mapper.Map<IEnumerable<STStatoEntitaVm>>(results);
            _logger.LogInformation("Retrieved {Count} StatoEntita records", vms.Count());
            return vms;
        }
    }
}
