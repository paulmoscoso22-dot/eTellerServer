using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.TotalicCassa;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.TotalicCassa.Queries.GetTotaliCassa
{
    public class GetTotaliCassaQueryHandler : IRequestHandler<GetTotaliCassaQuery, IEnumerable<TotalicCassaVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTotaliCassaQueryHandler> _logger;

        public GetTotaliCassaQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTotaliCassaQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TotalicCassaVm>> Handle(GetTotaliCassaQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with parameters CliID={CliID}, Data={Data}, CutID={CutID}, BraID={BraID}",
                nameof(GetTotaliCassaQuery), request.tocCliId, request.tocData, request.tocCutId, request.tocBraId);

            var totalicassas = await _unitOfWork.TotalicCassaSpRepository.GetTotaliCassaByClientIDAndDataAndCutID(
                request.tocCliId,
                request.tocData,
                request.tocCutId,
                request.tocBraId);

            var totalicassaVms = _mapper.Map<IEnumerable<TotalicCassaVm>>(totalicassas);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetTotaliCassaQuery), totalicassaVms?.Count() ?? 0);
            return totalicassaVms;
        }
    }
}
