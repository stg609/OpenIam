using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Core.Services.Abstractions;
using IdentityModel;
using Microsoft.Extensions.DependencyInjection;

namespace Charlie.OpenIam.Core.Models.Services
{
    public class OpenIamPermissionService : AbstractPermissionService
    {
        private readonly IServiceProvider _sp;

        public OpenIamPermissionService(IServiceProvider sp)
        {
            _sp = sp;
        }

        protected override async Task<UserRolePermissionDto> GetUserRolesAndPermissionsAsync(ClaimsPrincipal user, IEnumerable<string> clientIds = null)
        {
            using (var scope = _sp.CreateScope())
            {
                var userSrv = scope.ServiceProvider.GetRequiredService<IUserService>();
               
                string userId = user.FindFirst(JwtClaimTypes.Subject)?.Value;
                return await userSrv.GetRolesAndPermissionsAsync(userId, clientIds);
            }
        }
    }
}
