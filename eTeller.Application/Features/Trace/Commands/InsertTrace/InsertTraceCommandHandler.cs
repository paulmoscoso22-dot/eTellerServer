using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Trace.Commands.InsertTrace
{
    public class InsertTraceCommandHandler : IRequestHandler<InsertTraceCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InsertTraceCommandHandler> _logger;

        public InsertTraceCommandHandler(IUnitOfWork unitOfWork, ILogger<InsertTraceCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(InsertTraceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for User={TraUser}, FunCode={TraFunCode}, TabNam={TraTabNam}",
                nameof(InsertTraceCommand), request.TraUser, request.TraFunCode, request.TraTabNam);

            var result = await _unitOfWork.TraceRepository.InsertTrace(
                request.TraTime,
                request.TraUser,
                request.TraFunCode,
                request.TraSubFun,
                request.TraStation,
                request.TraTabNam,
                request.TraEntCode,
                request.TraRevTrxTrace,
                request.TraDes,
                request.TraExtRef,
                request.TraError
            );

            _logger.LogInformation("Handled {CommandName}, insert successful", nameof(InsertTraceCommand));
            
            return result;
        }
    }
}
