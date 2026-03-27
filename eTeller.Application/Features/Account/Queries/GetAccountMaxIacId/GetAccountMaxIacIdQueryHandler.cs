using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountMaxIacId
{
    public class GetAccountMaxIacIdQueryHandler : IRequestHandler<GetAccountMaxIacIdQuery, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAccountMaxIacIdQueryHandler> _logger;

        public GetAccountMaxIacIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAccountMaxIacIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(GetAccountMaxIacIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetAccountMaxIacIdQuery));
            var result = await _unitOfWork.AccountSpRepository.GetAccountMaxIacId();
            _logger.LogInformation("Handled {QueryName}, result={Result}", nameof(GetAccountMaxIacIdQuery), result);
            return result;
        }
    }
}
