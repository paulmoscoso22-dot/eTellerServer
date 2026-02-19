using AutoMapper;
using eTeller.Application.Features.StoreProcedures.AntirecAppearer.Mapping;
using eTeller.Application.Mappings.Currency;
using eTeller.Application.Mappings.Branch;
using eTeller.Application.Mappings.ST_CurrencyType;
using eTeller.Application.Mappings.ST_OperationType;
using eTeller.Application.Mappings.TotalicCassa;
using CurModel = eTeller.Domain.Models;
using eTeller.Domain.Models.StoredProcedure;
using eTeller.Domain.Models.View;

namespace eTeller.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CurModel.Account, AccountVm>();
            CreateMap<AntirecAppearerView, AntirecAppearerViewVm>();
            CreateMap<CurModel.Transaction, TransactionVm>();
            CreateMap<CurModel.TransactionMov, TransactionMovVm>();
            CreateMap<GiornaleAntiriciclaggio, GiornaleAntiriciclaggioVm>();
            CreateMap<CurModel.Currency, CurrencyVm>();
            CreateMap<CurModel.Branch, BranchVm>();
            CreateMap<CurModel.ST_CurrencyType, ST_CurrencyTypeVm>();
            CreateMap<CurModel.ST_OperationType, ST_OperationTypeVm>();
            CreateMap<CurModel.TotalicCassa, TotalicCassaVm>();
        }
    }
}

