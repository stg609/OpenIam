using System.Collections.Generic;

namespace Charlie.OpenIam.Abstraction.Dtos
{
    /// <summary>
    /// 角色模型
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Client id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 是否是管理员
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// 拥有的权限
        /// </summary>
        public IEnumerable<PermissionDto> Permissions { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedAt { get; set; }
    }
}
