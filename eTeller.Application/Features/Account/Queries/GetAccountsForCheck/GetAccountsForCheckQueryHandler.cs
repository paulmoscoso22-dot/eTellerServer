using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;


namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountsForCheck
{
    public class GetAccountsForCheckQueryHandler : IRequestHandler<GetAccountsForCheckQuery, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAccountsForCheckQueryHandler> _logger;

        public GetAccountsForCheckQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAccountsForCheckQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(GetAccountsForCheckQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with request {@Request}", nameof(GetAccountsForCheckQuery), request);
            var count = await _unitOfWork.AccountSpRepository.GetAccountForCheck(
                request.iacCutId,
                request.iacCurId,
                request.iacActId,
                request.iacCliCassa
                );
            _logger.LogInformation("Handled {QueryName}, count={Count}", nameof(GetAccountsForCheckQuery), count);
            return count;
        }
    }
}
