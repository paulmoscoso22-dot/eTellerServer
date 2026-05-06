using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.GestioneErrori;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.GestioneErrori.Queries.GetGestioneErroriById;

public class GetGestioneErroriByIdQueryHandler : IRequestHandler<GetGestioneErroriByIdQuery, GestioneErroriVm?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetGestioneErroriByIdQueryHandler> _logger;

    public GetGestioneErroriByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetGestioneErroriByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GestioneErroriVm?> Handle(GetGestioneErroriByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {QueryName} ErrId:{ErrId}", nameof(GetGestioneErroriByIdQuery), request.ErrId);

        var results = await _unitOfWork.Repository<ErrorCode>().GetAsync(e => e.ErrId == request.ErrId);
        var entity = results.FirstOrDefault();
        return entity is null ? null : _mapper.Map<GestioneErroriVm>(entity);
    }
}
