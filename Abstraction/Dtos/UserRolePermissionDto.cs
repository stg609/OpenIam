using System.Collections.Generic;

namespace Charlie.OpenIam.Abstraction.Dtos
{
    public class UserRolePermissionDto
    {
        /// <summary>
        /// 用户拥有的所有角色
        /// </summary>
        public IEnumerable<RoleDto> Roles { get; set; }

        /// <summary>
        /// 用户拥有的所有权限
        /// </summary>
        public IEnumerable<PermissionDto> Permissions { get; set; }
    }
}
