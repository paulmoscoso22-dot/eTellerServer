using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace eTeller.Application.Features.StoreProcedures.Transaction.Queries.GetTransactionWaitingForBef
{
    public class GetTransactionWaitingForBefQueryHandler : IRequestHandler<GetTransactionWaitingForBefQuery, IEnumerable<TransactionVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTransactionWaitingForBefQueryHandler> _logger;

        public GetTransactionWaitingForBefQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTransactionWaitingForBefQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TransactionVm>> Handle(GetTransactionWaitingForBefQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with parameters Cassa={Cassa}, DataDal={DataDal}, DataAl={DataAl}, Status={Status}, BraId={BraId}",
                nameof(GetTransactionWaitingForBefQuery), request.trxCassa, request.trxDataDal, request.trxDataAl, request.trxStatus, request.trxBraId);

            var transactions = await _unitOfWork.TransactionSpRepository.GetTransactionWaitingForBef(
                request.trxCassa,
                request.trxDataDal,
                request.trxDataAl,
                request.trxStatus,
                request.trxBraId);

            var transactionVms = _mapper.Map<IEnumerable<TransactionVm>>(transactions);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetTransactionWaitingForBefQuery), transactionVms?.Count() ?? 0);
            return transactionVms;
        }
    }
}
