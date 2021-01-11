using AutoMapper;
using Charlie.OpenIam.Core.Services.Dtos;
using Charlie.OpenIam.Web.Areas.Admin.ViewModels;

namespace Charlie.OpenIam.Web.Infra.Mappers
{
    public class RoleProfile:Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleNewViewModel, RoleNewDto>();
        }
    }
}
