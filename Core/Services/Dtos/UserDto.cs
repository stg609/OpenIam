using System.Collections.Generic;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Core.Models;

namespace Charlie.OpenIam.Core.Services.Dtos
{
    /// <summary>
    /// 用户 Dto
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string JobNo { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

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
        public Gender Gender { get; set; }

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
        /// 用户组织
        /// </summary>
        public List<OrganizationDto> Organizations { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public List<PermissionDto> Permissions { get; set; }
    }
}
