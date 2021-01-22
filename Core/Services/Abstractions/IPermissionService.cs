using System.Collections.Generic;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Core.Services.Dtos;

namespace Charlie.OpenIam.Core.Services.Abstractions
{
    /// <summary>
    /// 权限 服务
    /// </summary>
    /// <remarks>权限是可操控的最小单位</remarks>
    public interface IPermissionService
    {
        /// <summary>
        /// 增加权限
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<string> AddAsync(PermissionNewDto permission);

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <param name="url"></param>
        /// <param name="targetClientId"></param>
        /// <param name="type"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task<IEnumerable<PermissionDto>> GetAllsync(string name = null, string key = null, string url = null, string targetClientId = null, PermissionType? type = null, IEnumerable<string> allowedClientIds = null, string excludeRoleId = null, IEnumerable<string> excludePermIds = null);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task<PermissionDto> GetAsync(string id, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 更新权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task UpdateAsync(string id, PermissionUpdateDto model, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task RemoveAsync(IEnumerable<string> targetIds);

        /// <summary>
        /// 同步权限（覆盖），可用于子系统启动的时候同步
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task SyncPermissionsAsync(SyncPermissionDto permissions, IEnumerable<string> allowedClientIds = null);
    }
}
