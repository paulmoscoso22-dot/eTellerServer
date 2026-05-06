using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.ForceTrx;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetAllForceTrx
{
    public class GetAllForceTrxQueryHandler : IRequestHandler<GetAllForceTrxQuery, IEnumerable<ForceTrxVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllForceTrxQueryHandler> _logger;

        public GetAllForceTrxQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllForceTrxQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ForceTrxVm>> Handle(GetAllForceTrxQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with LanCode={LanCode}", nameof(GetAllForceTrxQuery), request.LanCode);

            var result = await _unitOfWork.ForceTrxRepository.GetAllAsync(request.LanCode);

            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetAllForceTrxQuery), result.Count());
            return _mapper.Map<IEnumerable<ForceTrxVm>>(result);
        }
    }
}
