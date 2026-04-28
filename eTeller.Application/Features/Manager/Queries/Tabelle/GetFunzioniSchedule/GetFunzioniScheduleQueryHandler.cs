using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager.Tabelle;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetFunzioniSchedule
{
    public class GetFunzioniScheduleQueryHandler : IRequestHandler<GetFunzioniScheduleQuery, IEnumerable<FunzioniScheduleVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetFunzioniScheduleQueryHandler> _logger;

        public GetFunzioniScheduleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetFunzioniScheduleQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<FunzioniScheduleVm>> Handle(GetFunzioniScheduleQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with NomeLike={NomeLike}, DesLike={DesLike}",
                nameof(GetFunzioniScheduleQuery), request.NomeLike, request.DesLike);

            var funzioni = await _unitOfWork.Repository<FUNZIONISCEDULE>().GetAsync(
                f => (request.NomeLike == null || f.FutFunname.Contains(request.NomeLike)) &&
                     (request.DesLike == null || f.FutDes.Contains(request.DesLike)));

            var result = _mapper.Map<IEnumerable<FunzioniScheduleVm>>(funzioni);

            _logger.LogInformation("Handled {QueryName}, returned {Count} items",
                nameof(GetFunzioniScheduleQuery), result?.Count() ?? 0);

            return result;
        }
    }
}
