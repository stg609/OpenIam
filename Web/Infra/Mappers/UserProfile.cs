using System.Linq;
using AutoMapper;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Services.Dtos;
using Charlie.OpenIam.Web.Areas.Admin.ViewModels;

namespace Charlie.OpenIam.Web.Infra.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(itm => itm.Phone, opt => opt.MapFrom(src => src.PhoneNumber));
            CreateMap<ApplicationUser, AdminUserDetailsDto>()
                .ForMember(itm => itm.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(itm => itm.Organizations, opt => opt.MapFrom(src => src.UserOrganizations.Select(itm => itm.Organization)));
            CreateMap<ApplicationUser, AdminUserDto>()
                .ForMember(itm => itm.Phone, opt => opt.MapFrom(src => src.PhoneNumber));
            CreateMap<UserNewViewModel, UserNewDto>();
        }
    }
}
