using System.ComponentModel.DataAnnotations;
using Charlie.OpenIam.Abstraction.Dtos;

namespace Charlie.OpenIam.Web.Areas.Admin.ViewModels
{
    public class PermissionNewViewModel
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public PermissionType Type { get; set; }

        /// <summary>
        /// Client id，除了内置权限，其他添加的权限都需要有归属的 client id
        /// </summary>
        [Required]
        public string ClientId { get; set; }

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
