using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Account;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.CustomerAccounts.Queries.GetCustomerAccountsByCliId
{
    public class GetCustomerAccountsByCliIdQueryHandler : IRequestHandler<GetCustomerAccountsByCliIdQuery, IEnumerable<CustomerAccountVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCustomerAccountsByCliIdQueryHandler> _logger;

        public GetCustomerAccountsByCliIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCustomerAccountsByCliIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CustomerAccountVm>> Handle(GetCustomerAccountsByCliIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with parameters CliId={CliId}",
                nameof(GetCustomerAccountsByCliIdQuery), request.CliId);

            var customerAccounts = await _unitOfWork.CustomerAccountSpRepository.GetCustomerAccountsByCliIdAsync(request.CliId);

            var customerAccountVms = _mapper.Map<IEnumerable<CustomerAccountVm>>(customerAccounts);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetCustomerAccountsByCliIdQuery), customerAccountVms?.Count() ?? 0);
            return customerAccountVms;
        }
    }
}
