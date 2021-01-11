using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Charlie.OpenIam.Abstraction
{
    /// <summary>
    /// 通用权限服务
    /// </summary>
    public interface IGeneralPermissionService
    {
        /// <summary>
        /// 判断用户是否有权限
        /// </summary>
        /// <param name="User">用户</param>
        /// <param name="permKey">权限的 key</param>
        /// <param name="isAdminRequired">该权限是否要求是管理员</param>
        /// <param name="clientIds">目标客户端的 ClientId</param>
        /// <returns></returns>
        Task<bool> HasPermissionAsync(ClaimsPrincipal User, string permKey, bool isAdminRequired = false, IEnumerable<string> clientIds = null);

        /// <summary>
        /// 同步子系统的权限到 Iam
        /// </summary>
        /// <param name="clientId">需要同步的clientId</param>
        /// <returns></returns>
        Task<bool> SyncPermissionsAsync(string authority, string clientId, string clientSecret);
    }
}
