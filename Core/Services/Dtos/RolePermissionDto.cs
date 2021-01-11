using System.Collections.Generic;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Common;

namespace Charlie.OpenIam.Core.Services.Dtos
{
    /// <summary>
    /// 用户拥有的角色权限 视图模型
    /// </summary>
    public class RolePermissionDto : IHasParentIdAndChildren<RolePermissionDto>
    {
        /// <summary>
        /// 编号，全局唯一
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 权限的 Key（同一Client中必须唯一）
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 人可读的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 类型： 0 菜单 2 Api 
        /// </summary>
        public PermissionType Type { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 顺序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 上级菜单
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 子级
        /// </summary>
        public List<RolePermissionDto> Children { get; set; } = new List<RolePermissionDto>();

        /// <summary>
        /// 是否拥有
        /// </summary>
        public bool IsOwned { get; set; }

    }
}
