using System;

namespace Charlie.OpenIam.Core.Models
{
    /// <summary>
    /// 用户权限
    /// </summary>
    public class UserPermission
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// 用户
        /// </summary>
        public ApplicationUser User { get; private set; }

        /// <summary>
        /// 权限编号
        /// </summary>
        public string PermissionId { get; private set; }

        /// <summary>
        /// 权限
        /// </summary>
        public Permission Permission { get; private set; }

        /// <summary>
        /// Permission 所属的角色编号，多个角色编号用英文逗号分隔
        /// </summary>
        /// <remarks>仅当 Action 为 exclude，用于标识 exclude 的 Permission 来自于哪里。</remarks>
        public string[] PermissionRoleIds { get; private set; }

        /// <summary>
        /// 操作
        /// </summary>
        public PermissionAction Action { get; private set; }

        protected UserPermission() { }

        public UserPermission(string userId, string permissionId, PermissionAction action, string[] permissionRoleIds = null)
        {
            UserId = userId;
            PermissionId = permissionId;
            Action = action;
            PermissionRoleIds = permissionRoleIds;
        }

        public void Update(PermissionAction? action = null, string[] permissionRoleIds = null)
        {
            Action = action ?? Action;
            PermissionRoleIds = permissionRoleIds ?? PermissionRoleIds;
        }
    }
}
