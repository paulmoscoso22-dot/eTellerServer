using AutoMapper;
using eTeller.Application.Contracts;
using CurModel = eTeller.Domain.Models;

namespace eTeller.Application.Mappings.Transaction
{
    public class GiornaleCassaTipoResolver : IValueResolver<CurModel.Transaction, TransactionGiornaleCassaVm, string?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GiornaleCassaTipoResolver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string? Resolve(CurModel.Transaction source, TransactionGiornaleCassaVm destination, string? destMember, ResolutionContext context)
        {
            return TransactionMappingHelper.ResolveTipo(source, _unitOfWork);
        }
    }
}
