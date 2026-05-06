using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.GestioneErrori;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.GestioneErrori.Queries.GetGestioneErrori;

public class GetGestioneErroriQueryHandler : IRequestHandler<GetGestioneErroriQuery, IEnumerable<GestioneErroriVm>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetGestioneErroriQueryHandler> _logger;

    public GetGestioneErroriQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetGestioneErroriQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<GestioneErroriVm>> Handle(GetGestioneErroriQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {QueryName} ErrId:{ErrId} Testo:{Testo}",
            nameof(GetGestioneErroriQuery), request.ErrId, request.TestoLike);

        var results = await _unitOfWork.Repository<ErrorCode>().GetAsync(
            e => (string.IsNullOrEmpty(request.ErrId) || e.ErrId == request.ErrId) &&
                 (string.IsNullOrEmpty(request.TestoLike) || e.ErrDescIt.Contains(request.TestoLike)));

        return _mapper.Map<IEnumerable<GestioneErroriVm>>(results);
    }
}
