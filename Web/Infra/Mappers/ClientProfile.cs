using AutoMapper;
using Charlie.OpenIam.Core.Services.Dtos;
using Charlie.OpenIam.Web.Areas.Admin.ViewModels;
using IdentityServer4.Models;

namespace Charlie.OpenIam.Web.Infra.Mappers
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientDto>()
                .ForMember(itm => itm.IsEnabled, opt => opt.MapFrom(src => src.Enabled));
            CreateMap<ClientNewViewModel, ClientNewDto>();
        }
    }
}
