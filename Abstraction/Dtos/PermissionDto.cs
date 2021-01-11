using System.Collections.Generic;
using Charlie.OpenIam.Common;

namespace Charlie.OpenIam.Abstraction.Dtos
{
    /// <summary>
    /// 权限模型
    /// </summary>
    public class PermissionDto : IHasParentIdAndChildren<PermissionDto>
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
        /// 权限类型
        /// </summary>
        public PermissionType Type { get; set; }

        /// <summary>
        /// 权限的 Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 父级权限
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 所属的 Client Id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 子级
        /// </summary>
        public List<PermissionDto> Children { get; set; } = new List<PermissionDto>();

        #region 用于 Type 为 View 的时候的额外字段

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
        public int? Order { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int? Level { get; set; }

        #endregion
    }
}
