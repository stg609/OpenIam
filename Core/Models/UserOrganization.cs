namespace Charlie.OpenIam.Core.Models
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public class UserOrganization
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
        /// 组织编号
        /// </summary>
        public string OrganizationId { get; private set; }

        /// <summary>
        /// 组织
        /// </summary>
        public Organization Organization { get; private set; }

        /// <summary>
        /// 是否是部门的负责人
        /// </summary>
        public bool IsCharger { get; private set; }

        protected UserOrganization()
        {

        }

        public UserOrganization(string orgId, string userId, bool isCharger)
        {
            OrganizationId = orgId;
            UserId = userId;
            IsCharger = isCharger;
        }
    }
}
