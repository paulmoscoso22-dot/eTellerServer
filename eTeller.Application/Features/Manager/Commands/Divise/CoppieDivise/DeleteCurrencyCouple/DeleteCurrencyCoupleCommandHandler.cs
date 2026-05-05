using System;
using System.Threading;
using System.Threading.Tasks;
using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Divise.CoppieDivise.DeleteCurrencyCouple
{
    public class DeleteCurrencyCoupleCommandHandler : IRequestHandler<DeleteCurrencyCoupleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCurrencyCoupleCommandHandler> _logger;

        public DeleteCurrencyCoupleCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCurrencyCoupleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteCurrencyCoupleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for {Cur1}/{Cur2} by {User}",
                nameof(DeleteCurrencyCoupleCommand), request.CucCur1, request.CucCur2, request.TraUser);

            var deleted = await _unitOfWork.CurrencyCoupleRepository.DeleteAsync(request.CucCur1, request.CucCur2);

            if (deleted)
            {
                // ITraceRepository exposes InsertTrace(...) according to provided signatures.
                await _unitOfWork.TraceRepository.InsertTrace(
                    DateTime.UtcNow, request.TraUser, "CURRENCY_COUPLE", "DELETE",
                    request.TraStation, "CURRENCY_COUPLE",
                    $"{request.CucCur1}_{request.CucCur2}",
                    string.Empty, $"Delete CurrencyCouple {request.CucCur1}/{request.CucCur2}",
                    string.Empty, false);
            }

            return deleted;
        }
    }
}
