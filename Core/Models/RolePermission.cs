namespace Charlie.OpenIam.Core.Models
{
    /// <summary>
    /// 角色权限实体
    /// </summary>
    public class RolePermission
    {
        /// <summary>
        /// 角色权限id
        /// </summary>
        public string RoleId { get; private set; }

        /// <summary>
        /// 角色权限所属角色id
        /// </summary>
        public ApplicationRole Role { get; private set; }

        /// <summary>
        /// 角色权限所属权限id
        /// </summary>
        public string PermissionId { get; private set; }

        /// <summary>
        /// 权限
        /// </summary>
        public Permission Permission { get; private set; }

        protected RolePermission() { }

        public RolePermission(string roleId, string permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
}
