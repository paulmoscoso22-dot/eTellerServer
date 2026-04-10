using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Personalisation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Personalisation.GetPersonalisation
{
    public class GetPersonalisationQueryHandler : IRequestHandler<GetPersonalisationQuery, IEnumerable<PersonalisationVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPersonalisationQueryHandler> _logger;

        public GetPersonalisationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetPersonalisationQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<PersonalisationVm>> Handle(GetPersonalisationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Handling {QueryName}", nameof(GetPersonalisationQuery));

                var personalisation = await _unitOfWork.Repository<Domain.Models.Personalisation>().GetAllAsync();

                var personalisationVms = _mapper.Map<IEnumerable<PersonalisationVm>>(personalisation);
                _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetPersonalisationQuery), personalisationVms.Count());

                return personalisationVms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {QueryName}", nameof(GetPersonalisationQuery));
                throw;
            }
        }
    }
}
