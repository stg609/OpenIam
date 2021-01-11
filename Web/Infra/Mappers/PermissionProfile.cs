using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Charlie.OpenIam.Core.Services.Dtos;
using Charlie.OpenIam.Web.Areas.Admin.ViewModels;

namespace Charlie.OpenIam.Web.Infra.Mappers
{
    public class PermissionProfile:Profile
    {
        public PermissionProfile()
        {
            CreateMap<PermissionNewViewModel, PermissionNewDto>();
        }
    }
}
