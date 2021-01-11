using Charlie.OpenIam.Core.Models;

namespace Charlie.OpenIam.Core.Services.Dtos
{
    /// <summary>
    /// 更新用户的视图模型
    /// </summary>
    public class UserUpdateDto
    {
        /// <summary>
        /// 工号
        /// </summary>
        public string JobNo { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender? Gender { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        public string HomeAddress { get; set; }

        /// <summary>
        /// 初始密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 用户所属的组织机构编号，多个组织用逗号分隔
        /// </summary>
        public string OrgIds { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
