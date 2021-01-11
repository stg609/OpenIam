using AutoMapper;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Services.Dtos;
using Charlie.OpenIam.Web.Areas.Admin.ViewModels;
using IdentityServer4.Models;

namespace Charlie.OpenIam.Web.Infra.Mappers
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap(typeof(PaginatedDto<>), typeof(PaginatedDto<>));
        }
    }
}
