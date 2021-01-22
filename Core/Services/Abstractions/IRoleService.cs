using System.Collections.Generic;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Core.Services.Dtos;

namespace Charlie.OpenIam.Core.Services.Abstractions
{
    /// <summary>
    /// 角色 服务
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// 增加角色
        /// </summary>
        /// <param name="role"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task AddAsync(RoleNewDto role, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="targetIds"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task RemoveAsync(IEnumerable<string> targetIds, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="role"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task UpdateAsync(string id, RoleUpdateDto role, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <param name="name"></param>
        /// <param name="clientId"></param>
        /// <param name="roleIds"></param>
        /// <param name="withPerms"></param>
        /// <param name="allowedClientIds"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        Task<PaginatedDto<RoleDto>> GetAllAsync(string name = null, string clientId = null, IEnumerable<string> roleIds = null, bool withPerms = false, IEnumerable<string> allowedClientIds = null, string excludeOrgId = null, IEnumerable<string> excludeRoleIds = null, int pageSize = 10, int pageIndex = 1);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="withPerms"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task<RoleDto> GetAsync(string id, bool withPerms = false, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 获取角色所属的权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="getAllPermissions"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task<IEnumerable<RolePermissionDto>> GetPermissionsAsync(string id, bool getAllPermissions = false, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 更新角色中的权限（覆盖操作）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task UpdatePermissionsAsync(string id, AssignPermissionDto model, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 增加权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task AddPermissionsAsync(string id, AssignPermissionDto model, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 移除权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task RemovePermissionsAsync(string id, AssignPermissionDto model, IEnumerable<string> allowedClientIds = null);
    }
}
