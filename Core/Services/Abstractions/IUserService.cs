using System.Collections.Generic;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction.Dtos;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Core.Services.Dtos;

namespace Charlie.OpenIam.Core.Services.Abstractions
{
    /// <summary>
    /// 用户 服务
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 工号是否唯一
        /// </summary>
        /// <returns></returns>
        Task<bool> IsJobNoUniqueAsync();

        /// <summary>
        /// 手机号是否唯一
        /// </summary>
        /// <returns></returns>
        Task<bool> IsPhoneUniqueAsync();

        /// <summary>
        /// 用户是否为管理员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<(bool IsSuperAdmin, bool IsAdmin)> IsAdminAsync(string id);

        /// <summary>
        /// 获取用户的角色及权限
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="clientIds">目标 ClientIds</param>
        /// <param name="includePermissionsInRole">角色数据中是否要返回所拥有的权限</param>
        /// <returns></returns>
        Task<UserRolePermissionDto> GetRolesAndPermissionsAsync(string userId, IEnumerable<string> allowedClientIds = null, bool includePermissionsInRole = false);

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="jobNo"></param>
        /// <param name="idcard"></param>
        /// <param name="phone"></param>
        /// <param name="email"></param>
        /// <param name="excludeOrgId"></param>
        /// <param name="isActive"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        Task<PaginatedDto<AdminUserDto>> GetAllAsync(string firstName = null, string lastName = null, string jobNo = null, string idcard = null, string phone = null, string email = null, string excludeOrgId = null, bool? isActive = null, int pageSize = 10, int pageIndex = 0);

        /// <summary>
        /// 获取某个用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AdminUserDetailsDto> GetAsync(string id = null, string phone = null, string jobNo = null);

        /// <summary>
        /// 获取用户的所有角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="getAll">是否获取所有角色（无论当前用户是否已经拥有）</param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task<IEnumerable<UserRoleDto>> GetRolesAsync(string id, bool getAll = false, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> CreateAsync(UserNewDto model);

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpdateAsync(string id, UserUpdateDto model);

        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task SwitchAsync(string id, ActiveUserDto model);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task RemoveAsync(string id);

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<string> ResetPwdAsync(string id);

        /// <summary>
        /// 给用户赋予权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permissions"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task AssignPermissionsAsync(string id, AssignPermissionToUserDto permissions, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 移除用户的权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permissionIds"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task RemovePermissionsAsync(string id, IEnumerable<string> permissionIds, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 赋予角色给用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task AssignRolesAsync(string id, AssignRoleToUserDto model, IEnumerable<string> allowedClientIds = null);
        Task<IEnumerable<OrganizationDto>> GetOrgsAsync(string userId);
    }
}
