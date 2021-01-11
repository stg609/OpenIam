using System.Collections.Generic;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction.Dtos;

namespace Charlie.OpenIam.Core.Models.Repositories
{
    /// <summary>
    /// 权限 仓储接口
    /// </summary>
    public interface IPermissionRepo
    {
        /// <summary>
        /// 判断权限是否存在
        /// </summary>
        /// <param name="id">权限编号</param>
        /// <param name="clientId">所属的 Client Id</param>
        /// <param name="key">权限的 Key</param>
        /// <returns></returns>
        Task<bool> IsExistedAsync(string id = null, string clientId = null, string key = null);

        /// <summary>
        /// 获取所有权限
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="key">Key</param>
        /// <param name="url">如果是菜单权限，则可以基于 url 进行查找</param>
        /// <param name="clientId">所属的 Client Id</param>
        /// <param name="type">权限类型</param>
        /// <param name="allowedClientIds">允许当前用户查看的 ClientIds</param>
        /// <returns></returns>
        Task<IEnumerable<Permission>> GetAllAsync(string name = null, string key = null, string url = null, string clientId = null, PermissionType? type = null, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 获取某个权限的详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isReadonly"></param>
        /// <returns></returns>
        Task<Permission> GetAsync(string id, bool isReadonly = true);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="permission"></param>
        void Add(Permission permission);

        /// <summary>
        /// 移除权限
        /// </summary>
        /// <param name="targetIds"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task RemoveAsync(IEnumerable<string> targetIds, IEnumerable<string> allowedClientIds = null);
    }
}
