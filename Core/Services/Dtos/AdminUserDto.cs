﻿using System.Collections.Generic;
using Charlie.OpenIam.Core.Models;

namespace Charlie.OpenIam.Core.Services.Dtos
{
    /// <summary>
    /// 用户视图模型
    /// </summary>
    public class AdminUserDto
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 内部系统的人员编号，可以是工号，必须能唯一关联用户
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
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

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
        /// 用户组织
        /// </summary>
        public List<string> Organizations { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 最后一次登陆的 Ip
        /// </summary>
        public string LastIp { get; set; }

        /// <summary>
        /// 最后一次登陆时间
        /// </summary>
        public string LastLoginAt { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedAt { get; set; }
    }
}
