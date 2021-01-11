using Microsoft.AspNetCore.Authorization;

namespace Charlie.OpenIam.Abstraction
{
    /// <summary>
    /// 权限的需求
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 所需要的权限
        /// </summary>
        public string Permission
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permission">所需的权限</param>
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
