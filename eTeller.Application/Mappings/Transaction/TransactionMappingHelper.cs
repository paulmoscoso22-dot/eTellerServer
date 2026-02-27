using eTeller.Application.Contracts;
using CurModel = eTeller.Domain.Models;

namespace eTeller.Application.Mappings.Transaction
{
    public static class TransactionMappingHelper
    {
        public static string? ResolveTipo(CurModel.Transaction source, IUnitOfWork unitOfWork)
        {
            if (string.IsNullOrEmpty(source.TrxOptId))
                return null;

            var optionRepository = unitOfWork.Repository<CurModel.ST_OperationType>();
            var operation = optionRepository.GetAllAsync().Result
                .FirstOrDefault(o => o.OptId == source.TrxOptId);

            return operation?.OptDes;
        }

        public static string? ResolveReport(CurModel.Transaction source, IUnitOfWork unitOfWork)
        {
            if (!string.IsNullOrEmpty(source.TrxCtoope))
                return source.TrxCtoope;

            if (string.IsNullOrEmpty(source.TrxCutId))
                return string.Empty;

            var currencyType = unitOfWork.ST_CurrencyTypeSpRepository
                .GetByCutID(source.TrxCutId).Result;

            return currencyType?.CutDes ?? string.Empty;
        }
    }
}
