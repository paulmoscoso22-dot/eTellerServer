using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.BookingRc;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetAccountTypes
{
    public class GetAccountTypesQueryHandler : IRequestHandler<GetAccountTypesQuery, IEnumerable<AccountTypeVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAccountTypesQueryHandler> _logger;

        public GetAccountTypesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAccountTypesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AccountTypeVm>> Handle(GetAccountTypesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetAccountTypesQuery));
            var result = await _unitOfWork.Repository<StAccountType>().GetAllAsync();
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetAccountTypesQuery), result.Count);
            return _mapper.Map<IEnumerable<AccountTypeVm>>(result);
        }
    }
}
