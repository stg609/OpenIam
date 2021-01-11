namespace Charlie.OpenIam.Core.Services.Dtos
{
    public class OrganizationRoleDto
    {
        /// <summary>
        /// 编号，全局唯一
        /// </summary>
        public string Id { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public bool IsAdmin { get; set; }

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// 是否拥有
        /// </summary>
        public bool IsOwned { get; set; }

    }
}
