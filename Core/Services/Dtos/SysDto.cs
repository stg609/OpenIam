using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace Charlie.OpenIam.Core.Models.Services.Dtos
{
    /// <summary>
    /// 系统信息 Dto
    /// </summary>
    public class SysDto
    {
        /// <summary>
        /// 工号是否唯一
        /// </summary>
        public bool IsJobNoUnique { get; set; }

        /// <summary>
        /// 用户手机号是否唯一
        /// </summary>
        public bool IsUserPhoneUnique { get; set; }

        /// <summary>
        /// 是否允许手机号密码登录
        /// </summary>
        public bool IsPhonePwdLoginEnabled { get; set; }

        /// <summary>
        /// 是否允许工号密码登录
        /// </summary>
        public bool IsJobNoPwdLoginEnabled { get; set; }

        /// <summary>
        /// 是否允许用户自己注册
        /// </summary>
        public bool IsRegisterUserEnabled { get; set; }

        /// <summary>
        /// 启用的外部扫码登录（如需启用，须要配置外部登录先）
        /// </summary>
        public string[] EnabledQrExternalLogins { get; set; }

        /// <summary>
        /// 所有可供选择的扫码外部登录方式
        /// </summary>
        public IEnumerable<AuthenticationScheme> AllQrExternalLogins { get; set; }

        /// <summary>
        /// 最后更新的时间
        /// </summary>
        public DateTime LastUpdatedAt
        {
            get; set;
        }
    }
}
