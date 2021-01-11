using System;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Common;
using Microsoft.AspNetCore.Authorization;

namespace Charlie.OpenIam.Sdk.Services
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IGeneralPermissionService _hasPermission;

        public PermissionHandler(IGeneralPermissionService hasPermission)
        {
            _hasPermission = hasPermission;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (requirement != null && !String.IsNullOrWhiteSpace(requirement.Permission))
            {
                var permsInfo = requirement.Permission.Split(Constants.ColonDelimiter);
                Boolean.TryParse(permsInfo[1], out bool isAdminRequired);
                if (await _hasPermission.HasPermissionAsync(context.User, permsInfo[0], isAdminRequired))
                {
                    context.Succeed(requirement);
                }
            }        
        }
    }
}
