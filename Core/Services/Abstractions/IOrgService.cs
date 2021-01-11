using System.Collections.Generic;
using System.Threading.Tasks;
using Charlie.OpenIam.Core.Services.Dtos;

namespace Charlie.OpenIam.Core.Services.Abstractions
{
    /// <summary>
    /// 组织 服务
    /// </summary>
    public interface IOrgService
    {
        /// <summary>
        /// 获取所有
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<IEnumerable<OrganizationDto>> GetAllAsync(string name = null);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<OrganizationDto> GetAsync(string id);

        /// <summary>
        /// 获取组织的默认角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="getAllRoles"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task<IEnumerable<OrganizationRoleDto>> GetRolesAsync(string id, bool getAllRoles = false, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 新增组织
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        string Add(OrganizationNewDto organization, string userId);

        /// <summary>
        /// 更新组织
        /// </summary>
        /// <param name="id"></param>
        /// <param name="organization"></param>
        /// <returns></returns>
        Task UpdateAsync(string id, OrganizationUpdateDto organization);

        /// <summary>
        /// 删除组织
        /// </summary>
        /// <param name="userId">当前用户</param>
        /// <param name="ids">要删除的组织编号</param>
        /// <returns>删除的组织编号</returns>
        Task<IEnumerable<string>> RemoveAsync(string userId, string ids);

        /// <summary>
        /// 新增默认角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task AddDefaultRolesAsync(string id, AssignRoleToOrgDto model, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 删除默认角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roleIds"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task DeleteDefaultRolesAsync(string id, IEnumerable<string> roleIds, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 更新默认角色（覆盖操作）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task UpdateDefaultRolesAsync(string id, AssignRoleToOrgDto model, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 获取组织下的所有用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<AdminUserDto>> GetUsersAsync(string id);

        /// <summary>
        /// 更新组织内的用户（覆盖操作）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpdateUsersAsync(string id, AssignUserToOrgDto model);

        /// <summary>
        /// 增加组织内的用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task AddUsersAsync(string id, AssignUserToOrgDto model);

        /// <summary>
        /// 删除组织内的用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        Task RemoveUsersAsync(string id, IEnumerable<string> userIds);
    }
}
