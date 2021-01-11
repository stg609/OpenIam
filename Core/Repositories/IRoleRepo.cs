using System.Collections.Generic;
using System.Threading.Tasks;
using Charlie.OpenIam.Common;

namespace Charlie.OpenIam.Core.Models.Repositories
{
    /// <summary>
    /// 角色 仓储
    /// </summary>
    public interface IRoleRepo
    {
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
        Task<PaginatedDto<ApplicationRole>> GetAllAsync(string name = null, string clientId = null, IEnumerable<string> roleIds = null, bool withPerms = false, IEnumerable<string> allowedClientIds = null, int pageSize = 10, int pageIndex = 1);

        /// <summary>
        /// 基于角色的名称获取所有角色
        /// </summary>
        /// <param name="names"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task<IEnumerable<ApplicationRole>> GetAllByNamesAsync(IEnumerable<string> names, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 基于角色的 Id 获取所有角色
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task<IEnumerable<ApplicationRole>> GetAllByIdsAsync(IEnumerable<string> ids, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 获取某个角色详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="withPerms"></param>
        /// <param name="isReadonly"></param>
        /// <returns></returns>
        Task<ApplicationRole> GetAsync(string id, bool withPerms = false, bool isReadonly = true);

        /// <summary>
        /// 判断某个角色是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<bool> IsExistedAsync(string name = null, string clientId = null);
    }
}
