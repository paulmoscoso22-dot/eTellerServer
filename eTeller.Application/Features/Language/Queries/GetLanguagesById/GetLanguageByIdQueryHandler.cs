using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Language;
using MediatR;
using Microsoft.Extensions.Logging;
using eTeller.Application.Exceptions;

namespace eTeller.Application.Features.Language.Queries.GetLanguagesById
{
    public class GetLanguageByIdQueryHandler : IRequestHandler<GetLanguageByIIdQuery, IEnumerable<STLanguageVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetLanguageByIdQueryHandler> _logger;

        public GetLanguageByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetLanguageByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<STLanguageVm>> Handle(GetLanguageByIIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for LanId: {LanId}", nameof(GetLanguageByIIdQuery), request.LanId);

            var repo = _unitOfWork.Repository<Domain.Models.ST_LANGUAGE>();
            var results = await repo.GetAsync(s => s.LanId == request.LanId);

            if (results == null || !results.Any())
            {
                _logger.LogWarning("No languages found for LanId: {LanId}", request.LanId);
                throw new NotFoundException(nameof(GetLanguageByIIdQuery), request.LanId);
            }

            var vms = _mapper.Map<IEnumerable<STLanguageVm>>(results);
            _logger.LogInformation("Retrieved {Count} languages for LanId: {LanId}", vms.Count(), request.LanId);
            return vms;
        }
    }
}
