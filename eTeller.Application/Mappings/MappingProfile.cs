using AutoMapper;
using eTeller.Application.Features.StoreProcedures.AntirecAppearer.Mapping;
using eTeller.Application.Mappings.Prelievo;
using eTeller.Application.Mappings.Currency;
using eTeller.Application.Mappings.Branch;
using eTeller.Application.Mappings.ST_CurrencyType;
using eTeller.Application.Mappings.ST_OperationType;
using eTeller.Application.Mappings.StCountry;
using eTeller.Application.Mappings.TotalicCassa;
using eTeller.Application.Mappings.Transaction;
using eTeller.Application.Mappings.Vigilanza;
using CurModel = eTeller.Domain.Models;
using CurModelSp = eTeller.Domain.Models.StoredProcedure;
using eTeller.Domain.Models.StoredProcedure;
using eTeller.Domain.Models.View;
using eTeller.Application.Mappings.Manager;

using eTeller.Application.Mappings.Account;
using eTeller.Application.Mappings.Manager;
using eTeller.Application.Mappings.Trace;
using eTeller.Application.Mappings.Client;
using eTeller.Application.Mappings.Tabella;
using eTeller.Application.Mappings.User;
using eTeller.Application.Mappings.Personalisation;
using CurModelTrace = eTeller.Domain.Models;
using eTeller.Domain.Common;
using eTeller.Application.Mappings.Language;
using eTeller.Application.Mappings.StatoEntita;

namespace eTeller.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CurModel.Account, AccountVm>();
            CreateMap<CurModel.CustomerAccount, CustomerAccountVm>();
            CreateMap<CurModel.Customers, CustomersVm>();
            CreateMap<AntirecAppearerView, AntirecAppearerViewVm>();
            CreateMap<CurModel.Transaction, TransactionVm>()
                .ForMember(dest => dest.Genere, opt => opt.MapFrom(src => string.Format("{0}/{1}", src.TrxUsrId, src.TrxCassa)))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom<TransactionTipoResolver>())
                .ForMember(dest => dest.Report, opt => opt.MapFrom<TransactionReportResolver>());
            CreateMap<CurModel.TransactionMov, TransactionMovVm>();

            // ST_COUNTRY mapping
            CreateMap<CurModel.ST_COUNTRY, StCountryVm>();

            // Giornale Cassa mapping
            CreateMap<CurModel.Transaction, TransactionGiornaleCassaVm>()
                .ForMember(dest => dest.Genere, opt => opt.MapFrom(src => string.Format("{0}/{1}", src.TrxUsrId, src.TrxCassa)))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom<GiornaleCassaTipoResolver>())
                .ForMember(dest => dest.Report, opt => opt.MapFrom<GiornaleCassaReportResolver>())
                .ForMember(dest => dest.BigliettiBanca, opt => opt.MapFrom(src => src.TrxImpctp.HasValue ? src.TrxImpctp.Value.ToString(FormatConstants.FormatAmount) : null))
                .ForMember(dest => dest.NonContanti, opt => opt.MapFrom(src => src.TrxCash != true && src.TrxImpope.HasValue ? src.TrxImpope.Value.ToString(FormatConstants.FormatAmount) : null))
                .ForMember(dest => dest.Contanti, opt => opt.MapFrom(src => src.TrxCash == true && src.TrxImpope.HasValue ? src.TrxImpope.Value.ToString(FormatConstants.FormatAmount) : null))
                .ForMember(dest => dest.ImpCHF, opt => opt.MapFrom(src => src.TrxImpctv.HasValue ? src.TrxImpctv.Value.ToString(FormatConstants.FormatAmount) : null))
                .ForMember(dest => dest.Stato, opt => opt.MapFrom<TransactionStatoResolver>());

            // Operazioni Annullate mapping
            CreateMap<CurModel.Transaction, TransactionOperationAnnullateVm>()
                .ForMember(dest => dest.Genere, opt => opt.MapFrom(src => string.Format("{0}/{1}", src.TrxUsrId, src.TrxCassa)))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom<OperazioniAnnullateTipoResolver>())
                .ForMember(dest => dest.Report, opt => opt.MapFrom<OperazioniAnnullateReportResolver>())
                .ForMember(dest => dest.HostTrace, opt => opt.MapFrom(src => src.TrxDatope.HasValue
                    ? TransactionHelper.GetHostTrace(src.TrxCassa, src.TrxDatope.Value, src.TrxDailySequence)
                    : null));

            CreateMap<GiornaleAntiriciclaggio, GiornaleAntiriciclaggioVm>();
            CreateMap<CurModel.Currency, CurrencyVm>();
            CreateMap<CurModel.Branch, BranchVm>();
            CreateMap<CurModel.ST_CurrencyType, ST_CurrencyTypeVm>();
            CreateMap<CurModel.ST_OperationType, ST_OperationTypeVm>();
            CreateMap<CurModel.TotalicCassa, TotalicCassaVm>();
            CreateMap<PrelievoView, PrelievoViewVm>();
            
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

            CreateMap<SpAntirecRules, SpAntirecRulesVm>();
            CreateMap<CurModel.AppearerAll, AppearerAllVm>();
            CreateMap<CurModel.HisAntirecAppearer, HisAntirecAppearerVm>();
            CreateMap<InfoAutorizzazioneUtente, InfoAutorizzazioneUtenteVm>();
            CreateMap<CurModel.SysFunctions, SysFunctionsVm>();
            CreateMap<CurModel.Trace, TraceVm>();
            CreateMap<CurModelTrace.ST_TRACE_FUNCTION, ST_TRACE_FUNCTIONVm>();
                CreateMap<CurModel.ST_STATOENTITA, STStatoEntitaVm>();
            CreateMap<CurModel.Client, ClientVm>();
            CreateMap<CurModel.Na_TabellaServVarchar, TabellaServVarcharVm>();
            CreateMap<CurModel.User, SysUsersActiveAndBlockedVm>();
            CreateMap<CurModel.User, SysUserByIdVm>();
            CreateMap<CurModel.User, InsertUserVm>();
            CreateMap<CurModel.ST_LANGUAGE, STLanguageVm>();
            CreateMap<CurModel.sys_ROLE, SysRoleVm>();
            CreateMap<CurModelSp.UserSelectRole, UserSelectRoleVm>();
            CreateMap<CurModelSp.FunctionRole, FunctionRoleVm>();
            CreateMap<UsersRoleFunction, UsersRoleFunctionVm>();
            CreateMap<USERS_AllAccess, USERS_AllAccessVm>();
            CreateMap<CurModel.Personalisation, PersonalisationVm>();
            CreateMap<CurModel.StFunAcctyp, StFunAcctypVm>();
            CreateMap<CurModel.sys_DEVICE, eTeller.Application.Mappings.CassePeriferiche.DeviceVm>();
        }
    }
}

