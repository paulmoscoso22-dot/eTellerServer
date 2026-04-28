using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Application.Mappings.Manager.Tabelle;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetFunzioniScheduleById
{
    public class GetFunzioniScheduleByIdQueryHandler : IRequestHandler<GetFunzioniScheduleByIdQuery, FunzioniScheduleVm?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetFunzioniScheduleByIdQueryHandler> _logger;

        public GetFunzioniScheduleByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetFunzioniScheduleByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<FunzioniScheduleVm?> Handle(GetFunzioniScheduleByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with FutId={FutId}", nameof(GetFunzioniScheduleByIdQuery), request.FutId);

            var results = await _unitOfWork.Repository<FUNZIONISCEDULE>()
                .GetAsync(f => f.FutId == request.FutId);

            var entity = results.FirstOrDefault();
            if (entity == null)
            {
                _logger.LogWarning("{QueryName}: FutId={FutId} not found", nameof(GetFunzioniScheduleByIdQuery), request.FutId);
                throw new NotFoundException(nameof(FUNZIONISCEDULE), request.FutId);
            }

            _logger.LogInformation("Handled {QueryName}, found FutId={FutId}", nameof(GetFunzioniScheduleByIdQuery), request.FutId);
            return _mapper.Map<FunzioniScheduleVm>(entity);
        }
    }
}
