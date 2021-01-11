namespace Charlie.OpenIam.Core.Services.Dtos
{
    /// <summary>
    /// 用户的角色 Dto
    /// </summary>
    public class UserRoleDto
    {
        /// <summary>
        /// 编号，全局唯一
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 角色姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 是否是管理员
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// 是否拥有
        /// </summary>
        public bool IsOwned { get; set; }

        /// <summary>
        /// 该角色来自于所从属的组织
        /// </summary>
        public bool IsBelongToOrg { get; set; }

    }
}
