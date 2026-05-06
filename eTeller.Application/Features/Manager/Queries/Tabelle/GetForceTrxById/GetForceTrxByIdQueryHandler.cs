using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.ForceTrx;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetForceTrxById
{
    public class GetForceTrxByIdQueryHandler : IRequestHandler<GetForceTrxByIdQuery, IEnumerable<ForceTrxVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetForceTrxByIdQueryHandler> _logger;

        public GetForceTrxByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetForceTrxByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ForceTrxVm>> Handle(GetForceTrxByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with TrfId={TrfId}", nameof(GetForceTrxByIdQuery), request.TrfId);

            var result = await _unitOfWork.ForceTrxRepository.GetByIdAsync(request.LanCode, request.TrfId);

            _logger.LogInformation("Handled {QueryName}", nameof(GetForceTrxByIdQuery));
            return _mapper.Map<IEnumerable<ForceTrxVm>>(result);
        }
    }
}
