using AutoMapper;
using eTeller.Application.Features.StoreProcedures.AntirecAppearer.Mapping;
using eTeller.Domain.Models;
using eTeller.Domain.Models.View;

namespace eTeller.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, AccountVm>();
            CreateMap<AntirecAppearerView, AntirecAppearerViewVm>();
        }
    }
}
