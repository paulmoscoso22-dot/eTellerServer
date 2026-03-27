using eTeller.Application.Contracts;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Account.Commands.UpdateAccount
{
    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAccountCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.AccountSpRepository.UpdateAccount(
                request.iacId,
                request.iacAccId,
                request.iacCutId,
                request.iacCurId,
                request.iacDes,
                request.iacActId,
                request.iacCliCassa,
                request.iacBraId,
                request.iacHostprefix
                );
            return result;
        }
    }
}
