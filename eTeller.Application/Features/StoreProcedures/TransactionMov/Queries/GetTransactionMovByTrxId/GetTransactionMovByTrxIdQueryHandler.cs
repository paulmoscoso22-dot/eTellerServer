using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace eTeller.Application.Features.StoreProcedures.TransactionMov.Queries.GetTransactionMovByTrxId
{
    public class GetTransactionMovByTrxIdQueryHandler : IRequestHandler<GetTransactionMovByTrxIdQuery, IEnumerable<TransactionMovVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTransactionMovByTrxIdQueryHandler> _logger;

        public GetTransactionMovByTrxIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTransactionMovByTrxIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TransactionMovVm>> Handle(GetTransactionMovByTrxIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with TrxId={TrxId}", nameof(GetTransactionMovByTrxIdQuery), request.trxId);

            var transactionMovs = await _unitOfWork.TransactionMovSpRepository.GetTransactionMovByTrxId(request.trxId);

            var transactionMovVms = _mapper.Map<IEnumerable<TransactionMovVm>>(transactionMovs);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetTransactionMovByTrxIdQuery), transactionMovVms?.Count() ?? 0);
            return transactionMovVms;
        }
    }
}
