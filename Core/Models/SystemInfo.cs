﻿using System;

namespace Charlie.OpenIam.Core.Models
{
    /// <summary>
    /// 系统信息
    /// </summary>
    public class SystemInfo : IAuditable
    {
        public string Id { get; private set; }

        /// <summary>
        /// 工号是否唯一
        /// </summary>
        public bool IsJobNoUnique { get; private set; }

        /// <summary>
        /// 用户手机号是否唯一
        /// </summary>
        public bool IsUserPhoneUnique { get; private set; }

        /// <summary>
        /// 是否允许手机号密码登录
        /// </summary>
        public bool IsPhonePwdLoginEnabled { get; private set; }

        /// <summary>
        /// 是否允许工号密码登录
        /// </summary>
        public bool IsJobNoPwdLoginEnabled { get; private set; }

        /// <summary>
        /// 是否允许用户自己注册
        /// </summary>
        public bool IsRegisterUserEnabled { get; private set; }

        public string CreatedBy
        {
            get; private set;
        }

        public DateTime CreatedAt
        {
            get; private set;
        }

        public string LastUpdatedBy
        {
            get; private set;
        }

        public DateTime LastUpdatedAt
        {
            get; private set;
        }

        protected SystemInfo()
        {

        }

        public SystemInfo(bool isJobNoUnique = false, bool isPhoneUnique = false, bool isPhonePwdLoginEnabled = false, bool isJobNoPwdLoginEnabled = false, bool isRegisterUserEnabled = false)
        {
            Id = Guid.NewGuid().ToString();
            IsJobNoUnique = isJobNoUnique;
            IsUserPhoneUnique = isPhoneUnique;
            IsJobNoPwdLoginEnabled = isJobNoPwdLoginEnabled;
            IsPhonePwdLoginEnabled = isPhonePwdLoginEnabled;
            IsRegisterUserEnabled = isRegisterUserEnabled;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="isJobNoUnique"></param>
        /// <param name="isPhoneUnique"></param>
        /// <param name="isPhonePwdLoginEnabled"></param>
        /// <param name="isJobNoPwdLoginEnabled"></param>
        /// <param name="isRegisterUserEnabled"></param>
        public void Update(bool? isJobNoUnique = null, bool? isPhoneUnique = null, bool? isPhonePwdLoginEnabled = null, bool? isJobNoPwdLoginEnabled = null, bool? isRegisterUserEnabled = null)
        {
            IsJobNoUnique = isJobNoUnique ?? IsJobNoUnique;
            IsUserPhoneUnique = isPhoneUnique ?? IsUserPhoneUnique;
            IsJobNoPwdLoginEnabled = isJobNoPwdLoginEnabled ?? IsJobNoPwdLoginEnabled;
            IsPhonePwdLoginEnabled = isPhonePwdLoginEnabled ?? IsPhonePwdLoginEnabled;
            IsRegisterUserEnabled = isRegisterUserEnabled ?? IsRegisterUserEnabled;
        }
    }
}
