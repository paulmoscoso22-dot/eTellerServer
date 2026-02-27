using AutoMapper;
using eTeller.Application.Contracts;
using CurModel = eTeller.Domain.Models;

namespace eTeller.Application.Mappings.Transaction
{
    public class OperazioniAnnullateTipoResolver : IValueResolver<CurModel.Transaction, TransactionOperationAnnullateVm, string?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public OperazioniAnnullateTipoResolver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string? Resolve(CurModel.Transaction source, TransactionOperationAnnullateVm destination, string? destMember, ResolutionContext context)
        {
            return TransactionMappingHelper.ResolveTipo(source, _unitOfWork);
        }
    }
}
