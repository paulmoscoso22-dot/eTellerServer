using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Vigilanza;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetAppearerAllByAraId
{
    public class GetAppearerAllByAraIdQueryHandler : IRequestHandler<GetAppearerAllByAraIdQuery, AppearerAllVm?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAppearerAllByAraIdQueryHandler> _logger;

        public GetAppearerAllByAraIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAppearerAllByAraIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AppearerAllVm?> Handle(GetAppearerAllByAraIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with AraId={AraId}",
                nameof(GetAppearerAllByAraIdQuery), request.AraId);

            var appearer = await _unitOfWork.VigilanzaRepository.GetAppearerAllByAraId(request.AraId);

            if (appearer == null)
            {
                _logger.LogInformation("Handled {QueryName}, no record found for AraId={AraId}", 
                    nameof(GetAppearerAllByAraIdQuery), request.AraId);
                return null;
            }

            var appearerVm = _mapper.Map<AppearerAllVm>(appearer);
            _logger.LogInformation("Handled {QueryName}, returned record for AraId={AraId}", 
                nameof(GetAppearerAllByAraIdQuery), request.AraId);
            
            return appearerVm;
        }
    }
}
