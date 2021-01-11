using AutoMapper;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Services.Dtos;

namespace Charlie.OpenIam.Web.Infra.Mappers
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<Organization, OrganizationDto>();
        }
    }
}
