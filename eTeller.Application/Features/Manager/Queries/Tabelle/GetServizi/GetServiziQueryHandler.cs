using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager.Tabelle;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetServizi
{
    public class GetServiziQueryHandler : IRequestHandler<GetServiziQuery, IEnumerable<ServiziVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetServiziQueryHandler> _logger;

        public GetServiziQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetServiziQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ServiziVm>> Handle(GetServiziQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetServiziQuery));

            var servizi = await _unitOfWork.Repository<SERVIZI>().GetAllAsync();
            var result = _mapper.Map<IEnumerable<ServiziVm>>(servizi);

            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetServiziQuery), result?.Count() ?? 0);

            return result;
        }
    }
}
