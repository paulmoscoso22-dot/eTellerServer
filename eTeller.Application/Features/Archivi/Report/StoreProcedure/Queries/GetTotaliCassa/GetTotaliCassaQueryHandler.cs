using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.TotalicCassa;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Archivi.Report.StoreProcedure.Queries.GetTotaliCassa
{
    public class GetTotaliCassaQueryHandler : IRequestHandler<GetSpTotaliCassaQuery, IEnumerable<TotalicCassaVm>>
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

        public async Task<IEnumerable<TotalicCassaVm>> Handle(GetSpTotaliCassaQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with parameters CliID={CliID}, Data={Data}, CutID={CutID}, BraID={BraID}",
                nameof(GetSpTotaliCassaQuery), request.tocCliId, request.tocData, request.tocCutId, request.tocBraId);

            var totalicassas = await _unitOfWork.TotalicCassaSpRepository.GetTotaliCassaByClientIDAndDataAndCutID(
                request.tocCliId,
                request.tocData,
                request.tocCutId,
                request.tocBraId);

            var totalicassaVms = _mapper.Map<IEnumerable<TotalicCassaVm>>(totalicassas);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetSpTotaliCassaQuery), totalicassaVms?.Count() ?? 0);
            return totalicassaVms;
        }
    }
}
