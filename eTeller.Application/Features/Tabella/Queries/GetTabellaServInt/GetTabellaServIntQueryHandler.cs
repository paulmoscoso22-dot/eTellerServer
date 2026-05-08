using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Tabella;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Tabella.Queries.GetTabellaServInt
{
    public class GetTabellaServIntQueryHandler : IRequestHandler<GetTabellaServIntQuery, IEnumerable<TabellaServIntVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTabellaServIntQueryHandler> _logger;

        public GetTabellaServIntQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTabellaServIntQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TabellaServIntVm>> Handle(GetTabellaServIntQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with NomeTabella: {NomeTabella}, Id: {Id}, DesLike: {DesLike}",
                nameof(GetTabellaServIntQuery), request.NomeTabella, request.Id, request.DesLike);

            var results = await _unitOfWork.TabellaRepository.GetTabellaServInt(
                request.NomeTabella,
                request.Id,
                request.DesLike
            );

            var vms = _mapper.Map<IEnumerable<TabellaServIntVm>>(results);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetTabellaServIntQuery), vms?.Count() ?? 0);

            return vms;
        }
    }
}
