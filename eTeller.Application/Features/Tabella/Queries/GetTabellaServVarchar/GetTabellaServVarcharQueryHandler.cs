using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Tabella;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Tabella.Queries.GetTabellaServVarchar
{
    public class GetTabellaServVarcharQueryHandler : IRequestHandler<GetTabellaServVarcharQuery, IEnumerable<TabellaServVarcharVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTabellaServVarcharQueryHandler> _logger;

        public GetTabellaServVarcharQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTabellaServVarcharQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TabellaServVarcharVm>> Handle(GetTabellaServVarcharQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with NomeTabella: {NomeTabella}, Id: {Id}, DesLike: {DesLike}",
                nameof(GetTabellaServVarcharQuery), request.NomeTabella, request.Id, request.DesLike);

            var results = await _unitOfWork.TabellaRepository.GetTabellaServVarchar(
                request.NomeTabella,
                request.Id,
                request.DesLike
            );

            var vms = _mapper.Map<IEnumerable<TabellaServVarcharVm>>(results);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetTabellaServVarcharQuery), vms?.Count() ?? 0);

            return vms;
        }
    }
}
