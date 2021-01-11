using System.ComponentModel.DataAnnotations;

namespace Charlie.OpenIam.Web.Areas.Admin.ViewModels
{
    public class RoleNewViewModel
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// 是否是当前客户端的管理员
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
