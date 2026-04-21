using AutoMapper;
using eTeller.Domain.Models;
using SysUsersUseClient = eTeller.Domain.Models.SysUsersUseClient;
using eTeller.Application.Mappings.User;

namespace eTeller.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<SysUsersUseClient, SysUsersUseClientVm>();
        }
    }
}
