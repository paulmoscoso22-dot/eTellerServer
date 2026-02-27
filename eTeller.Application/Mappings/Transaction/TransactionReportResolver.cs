using AutoMapper;
using eTeller.Application.Contracts;
using CurModel = eTeller.Domain.Models;

namespace eTeller.Application.Mappings.Transaction
{
    public class TransactionReportResolver : IValueResolver<CurModel.Transaction, TransactionVm, string?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionReportResolver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string? Resolve(CurModel.Transaction source, TransactionVm destination, string? destMember, ResolutionContext context)
        {
            return TransactionMappingHelper.ResolveReport(source, _unitOfWork);
        }
    }
}
