namespace Charlie.OpenIam.Core.Models
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public class UserRole
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
        /// 角色编号
        /// </summary>
        public string RoleId { get; private set; }

        /// <summary>
        /// 角色
        /// </summary>
        public ApplicationRole Role { get; private set; }
    }
}
