using AutoMapper;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Models.Services.Dtos;

namespace Charlie.OpenIam.Web.Infra.Mappers
{
    public class SysProfile : Profile
    {
        public SysProfile()
        {
            CreateMap<SystemInfo, SysDto>();
        }
    }
}
