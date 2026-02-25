using AutoMapper;
using eTeller.Application.Features.StoreProcedures.AntirecAppearer.Mapping;
using eTeller.Application.Mappings.Currency;
using eTeller.Application.Mappings.Branch;
using eTeller.Application.Mappings.ST_CurrencyType;
using eTeller.Application.Mappings.ST_OperationType;
using eTeller.Application.Mappings.TotalicCassa;
using eTeller.Application.Mappings.Vigilanza;
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
            
            // Vigilanza mappings
            CreateMap<SpTransactionGiornaleAntiriciclagio, SpTransactionGiornaleAntiriciclagioVm>()
                .ForMember(dest => dest.TrxDate, opt => opt.MapFrom(src => src.TrxDatope))
                .ForMember(dest => dest.TrxReport, opt => opt.MapFrom(src => src.TrxNumrel))
                .ForMember(dest => dest.TrxCurId, opt => opt.MapFrom(src => src.TrxDivope))
                .ForMember(dest => dest.TrxAmount, opt => opt.MapFrom(src => src.TrxImpope))
                .ForMember(dest => dest.TrxRate, opt => opt.MapFrom(src => src.TrxExcrat))
                .ForMember(dest => dest.AppearerName, opt => opt.MapFrom(src => src.AraName))
                .ForMember(dest => dest.CutDes, opt => opt.MapFrom(src => string.Format("{0}/{1}", src.TrxUsrId, src.TrxCassa)))
                .ForMember(dest => dest.OptDes, opt => opt.MapFrom<OperationTypeResolver>())
                .ForMember(dest => dest.BeneficiaryName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.AraName) ? "" : src.AraName))
                .ForMember(dest => dest.StaDes, opt => opt.MapFrom<TransactionStatusResolver>());
        }
    }
}

