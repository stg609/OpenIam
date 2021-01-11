using System;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Charlie.OpenIam.Core.Models.Services
{
    /// <summary>
    /// IdentityServer 中对 Permission Requirement 的 Handler
    /// </summary>
    public class IdentityServerPermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGeneralPermissionService _permissionService;

        public IdentityServerPermissionHandler(IHttpContextAccessor httpContextAccessor, IGeneralPermissionService permissionService)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            try
            {
                if (requirement != null && !String.IsNullOrWhiteSpace(requirement.Permission))
                {
                    var permsInfo = requirement.Permission.Split(":");
                    Boolean.TryParse(permsInfo[1], out bool isAdminRequired);
                    if(await _permissionService.HasPermissionAsync(context.User, permsInfo[0], isAdminRequired))
                    {
                        context.Succeed(requirement);
                    }                   
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
