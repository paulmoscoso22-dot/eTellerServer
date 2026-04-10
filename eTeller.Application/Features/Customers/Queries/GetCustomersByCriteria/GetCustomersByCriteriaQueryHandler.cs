using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Customers.Queries.GetCustomersByCriteria
{
    public class GetCustomersByCriteriaQueryHandler : IRequestHandler<GetCustomersByCriteriaQuery, IEnumerable<CustomersVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCustomersByCriteriaQueryHandler> _logger;

        public GetCustomersByCriteriaQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCustomersByCriteriaQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CustomersVm>> Handle(GetCustomersByCriteriaQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with parameters CliId={CliId}, Descrizione={Descrizione}",
                nameof(GetCustomersByCriteriaQuery), request.CliId, request.Descrizione);

            var customers = await _unitOfWork.CustomersRepository.GetCustomersByCriteriaAsync(
                request.CliId,
                request.Descrizione);

            var customersVms = _mapper.Map<IEnumerable<CustomersVm>>(customers);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetCustomersByCriteriaQuery), customersVms?.Count() ?? 0);
            return customersVms;
        }
    }
}
