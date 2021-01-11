using System;
using Charlie.OpenIam.Common;
using Microsoft.AspNetCore.Authorization;

namespace Charlie.OpenIam.Abstraction
{
    /// <summary>
    /// 用于声明方法或类型必须具备权限才能访问
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="permission">权限的Key</param>
        /// <param name="isAdminOnly">是否必须是管理员角色</param>
        /// <param name="desc">对该权限的描述</param>
        public HasPermissionAttribute(string permission, bool isAdminOnly = false, string desc = null)
        {
            Policy = String.Join(Constants.ColonDelimiter, permission, isAdminOnly.ToString().ToLower(), desc);
        }
    }
}
