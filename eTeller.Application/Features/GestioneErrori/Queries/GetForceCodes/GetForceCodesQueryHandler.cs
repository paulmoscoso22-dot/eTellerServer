using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.GestioneErrori;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.GestioneErrori.Queries.GetForceCodes;

public class GetForceCodesQueryHandler : IRequestHandler<GetForceCodesQuery, IEnumerable<ForceCodeVm>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetForceCodesQueryHandler> _logger;

    public GetForceCodesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetForceCodesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ForceCodeVm>> Handle(GetForceCodesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {QueryName}", nameof(GetForceCodesQuery));

        var results = await _unitOfWork.Repository<StForceCode>().GetAllAsync();
        return _mapper.Map<IEnumerable<ForceCodeVm>>(results);
    }
}
