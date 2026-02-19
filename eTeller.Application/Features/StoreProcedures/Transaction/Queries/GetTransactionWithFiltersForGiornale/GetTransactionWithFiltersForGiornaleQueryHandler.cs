using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace eTeller.Application.Features.StoreProcedures.Transaction.Queries.GetTransactionWithFiltersForGiornale
{
    public class GetTransactionWithFiltersForGiornaleQueryHandler : IRequestHandler<GetTransactionWithFiltersForGiornaleQuery, IEnumerable<TransactionVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTransactionWithFiltersForGiornaleQueryHandler> _logger;

        public GetTransactionWithFiltersForGiornaleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTransactionWithFiltersForGiornaleQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TransactionVm>> Handle(GetTransactionWithFiltersForGiornaleQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with parameters Cassa={Cassa}, DataDal={DataDal}, DataAl={DataAl}, Status={Status}, BraId={BraId}",
                nameof(GetTransactionWithFiltersForGiornaleQuery), request.trxCassa, request.trxDataDal, request.trxDataAl, request.trxStatus, request.trxBraId);

            var transactions = await _unitOfWork.TransactionSpRepository.GetTransactionWithFiltersForGiornale(
                request.trxCassa,
                request.trxDataDal,
                request.trxDataAl,
                request.trxStatus,
                request.trxBraId);

            var transactionVms = _mapper.Map<IEnumerable<TransactionVm>>(transactions);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetTransactionWithFiltersForGiornaleQuery), transactionVms?.Count() ?? 0);
            return transactionVms;
        }
    }
}
