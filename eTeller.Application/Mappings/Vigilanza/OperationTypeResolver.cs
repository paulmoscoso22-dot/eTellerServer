using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Domain.Models.StoredProcedure;
using StOptModel = eTeller.Domain.Models;

namespace eTeller.Application.Mappings.Vigilanza
{
    public class OperationTypeResolver : IValueResolver<SpTransactionGiornaleAntiriciclagio, SpTransactionGiornaleAntiriciclagioVm, string?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public OperationTypeResolver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string? Resolve(SpTransactionGiornaleAntiriciclagio source, SpTransactionGiornaleAntiriciclagioVm destination, string? destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.TrxOptId))
                return null;

            var optionRepository = _unitOfWork.Repository<StOptModel.ST_OperationType>();
            var operation =  optionRepository.GetAllAsync().Result
                .FirstOrDefault(o => o.OptId == source.TrxOptId);

            return operation?.OptDes;
        }
    }
}
