using System.ComponentModel.DataAnnotations;

namespace Charlie.OpenIam.Web.Areas.Admin.ViewModels
{
    /// <summary>
    /// 新增角色 视图模型
    /// </summary>
    public class RoleNewViewModel
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 所属的 Client Id
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// 是否是当前客户端的管理员
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
