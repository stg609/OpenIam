using System;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Charlie.OpenIam.Core.Models.Services
{
    /// <summary>
    /// OpenIam 中对 Permission Requirement 的 Handler
    /// </summary>
    public class OpenIamPermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGeneralPermissionService _permissionService;
        private readonly int _permKeyIndex = 0;
        private readonly int _isAdminIndex = 1;

        public OpenIamPermissionHandler(IHttpContextAccessor httpContextAccessor, IGeneralPermissionService permissionService)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (requirement != null && !String.IsNullOrWhiteSpace(requirement.Permission))
            {
                var permsInfo = requirement.Permission.Split(Constants.ColonDelimiter);
                Boolean.TryParse(permsInfo[_isAdminIndex], out bool isAdminRequired);
                if (await _permissionService.HasPermissionAsync(context.User, permsInfo[_permKeyIndex], isAdminRequired))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
