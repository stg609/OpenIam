using System.ComponentModel.DataAnnotations;

namespace Charlie.OpenIam.Web.ViewModels
{
    public class UpdatePwdViewModel
    {
        /// <summary>
        /// 原始密码
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string OldPwd { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string NewPwd { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPwd))]
        public string ConfirmNewPwd { get; set; }
    }
}
