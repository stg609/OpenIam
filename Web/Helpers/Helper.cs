using System;
using System.Security.Claims;
using Charlie.OpenIam.Common;

namespace Charlie.OpenIam.Web.Helpers
{
    /// <summary>
    /// 帮助类
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// 是否是超级管理员（可以管理 所有 Client)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsSuperAdmin(this ClaimsPrincipal user)
        {
            string isSuperClaim = user.FindFirst(Constants.SUPERADMIN_CLAIM_TYPE)?.Value;
            Boolean.TryParse(isSuperClaim, out bool isSuper);

            return isSuper;
        }
    }
}
