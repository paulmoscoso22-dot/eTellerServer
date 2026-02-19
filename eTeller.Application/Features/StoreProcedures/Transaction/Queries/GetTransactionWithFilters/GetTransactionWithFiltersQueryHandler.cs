using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace eTeller.Application.Features.StoreProcedures.Transaction.Queries.GetTransactionWithFilters
{
    public class GetTransactionWithFiltersQueryHandler : IRequestHandler<GetTransactionWithFiltersQuery, IEnumerable<TransactionVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTransactionWithFiltersQueryHandler> _logger;

        public GetTransactionWithFiltersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTransactionWithFiltersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TransactionVm>> Handle(GetTransactionWithFiltersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with parameters Cassa={Cassa}, DataDal={DataDal}, DataAl={DataAl}, Status={Status}, BraId={BraId}",
                nameof(GetTransactionWithFiltersQuery), request.trxCassa, request.trxDataDal, request.trxDataAl, request.trxStatus, request.trxBraId);

            var transactions = await _unitOfWork.TransactionSpRepository.GetTransactionWithFilters(
                request.trxCassa,
                request.trxDataDal,
                request.trxDataAl,
                request.trxStatus,
                request.trxBraId);

            var transactionVms = _mapper.Map<IEnumerable<TransactionVm>>(transactions);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetTransactionWithFiltersQuery), transactionVms?.Count() ?? 0);
            return transactionVms;
        }
    }
}
