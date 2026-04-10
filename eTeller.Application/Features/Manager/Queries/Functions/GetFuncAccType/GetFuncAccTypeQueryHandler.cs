using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Functions.GetFuncAccType
{
    public class GetFuncAccTypeQueryHandler : IRequestHandler<GetFuncAccTypeQuery, IEnumerable<StFunAcctypVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetFuncAccTypeQueryHandler> _logger;

        public GetFuncAccTypeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetFuncAccTypeQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<StFunAcctypVm>> Handle(GetFuncAccTypeQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetFuncAccTypeQuery));
            var stFunAcctyp = await _unitOfWork.Repository<StFunAcctyp>().GetAllAsync();
            var result = _mapper.Map<IEnumerable<StFunAcctypVm>>(stFunAcctyp);
            if (result == null) {
                _logger.LogError("La ricerca no ha prodotto nessnun resultato");
                throw new Exception("La ricerca no ha prodotto nessnun resultato");
            }     
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetFuncAccTypeQuery), result.Count());
            return result;
        }
    }
}