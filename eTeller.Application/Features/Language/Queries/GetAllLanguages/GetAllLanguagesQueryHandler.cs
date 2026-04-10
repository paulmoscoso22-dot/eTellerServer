using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Application.Mappings.Language;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Language.Queries.GetAllLanguages
{
    public class GetAllLanguagesQueryHandler : IRequestHandler<GetAllLanguagesQuery, IEnumerable<STLanguageVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllLanguagesQueryHandler> _logger;

        public GetAllLanguagesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllLanguagesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<STLanguageVm>> Handle(GetAllLanguagesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetAllLanguagesQuery));

            var repo = _unitOfWork.Repository<Domain.Models.ST_LANGUAGE>();
            var results = await repo.GetAllAsync();

            if (results == null || !results.Any())
            {
                _logger.LogWarning("No languages found");
                throw new NotFoundException(nameof(GetAllLanguagesQuery), "none");
            }

            var vms = _mapper.Map<IEnumerable<STLanguageVm>>(results);
            _logger.LogInformation("Retrieved {Count} languages", vms.Count());
            return vms;
        }
    }
}
