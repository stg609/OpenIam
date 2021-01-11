namespace Charlie.OpenIam.Core.Models
{
    /// <summary>
    /// 组织默认角色
    /// </summary>
    public class OrganizationRole
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
        /// 组织编号
        /// </summary>
        public string OrganizationId { get; private set; }

        /// <summary>
        /// 组织
        /// </summary>
        public Organization Organization { get; private set; }

        protected OrganizationRole() { }

        public OrganizationRole(string orgId, string roleId)
        {
            OrganizationId = orgId;
            RoleId = roleId;
        }
    }
}