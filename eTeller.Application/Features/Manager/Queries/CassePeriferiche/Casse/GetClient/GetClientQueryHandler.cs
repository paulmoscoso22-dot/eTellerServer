using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Client;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Features.Manager.Queries.CassePeriferiche.Casse.GetClient
{
    public class GetClientQueryHandler : IRequestHandler<GetClientQuery, IEnumerable<ClientVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetClientQueryHandler> _logger;

        public GetClientQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetClientQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ClientVm>> Handle(GetClientQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetClientQuery));

            var repository = _unitOfWork.Repository<Client>();
            var results = await repository.GetAllAsync();

            var vms = _mapper.Map<IEnumerable<ClientVm>>(results);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetClientQuery), vms?.Count() ?? 0);

            return vms;
        }
    }
}
