using AutoMapper;
using eTeller.Application.Contracts;
using StOptModel = eTeller.Domain.Models;

namespace eTeller.Application.Mappings.Transaction
{
    public class TransactionTipoResolver : IValueResolver<StOptModel.Transaction, TransactionVm, string?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionTipoResolver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string? Resolve(StOptModel.Transaction source, TransactionVm destination, string? destMember, ResolutionContext context)
        {
            return TransactionMappingHelper.ResolveTipo(source, _unitOfWork);
        }
    }
}
