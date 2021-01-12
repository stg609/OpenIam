using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Charlie.OpenIam.Web.Areas.Admin.ViewModels
{
    /// <summary>
    /// 同步权限 模型
    /// </summary>
    public class SyncPermissionViewModel
    {
        /// <summary>
        /// Client id，除了内置权限，其他添加的权限都需要有归属的 client id
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// 权限集合
        /// </summary>
        public IEnumerable<PermissionNewViewModel> Permissions { get; set; }
    }
}
