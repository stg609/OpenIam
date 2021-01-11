using System.ComponentModel.DataAnnotations;
using Charlie.OpenIam.Abstraction.Dtos;

namespace Charlie.OpenIam.Core.Services.Dtos
{
    /// <summary>
    /// 更新权限 Dto
    /// </summary>
    public class PermissionUpdateDto
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 权限类型
        /// </summary>
        public PermissionType? Type { get; set; }

        /// <summary>
        /// 父级权限
        /// </summary>
        public string ParentId { get; set; }

        #region 用于 Type 为 View 的时候的额外字段

        /// <summary>
        /// 请求地址
        /// </summary>
        [Url]
        public string Url { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [Url]
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
