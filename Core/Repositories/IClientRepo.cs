using System.Collections.Generic;
using System.Threading.Tasks;
using Charlie.OpenIam.Common;
using IdentityServer4.Models;

namespace Charlie.OpenIam.Core.Models.Repositories
{
    /// <summary>
    /// 客户端 仓储接口
    /// </summary>
    public interface IClientRepo
    {
        /// <summary>
        /// 获取所有客户端
        /// </summary>
        /// <param name="name">客户端名称</param>
        /// <param name="clientId">客户端 id</param>
        /// <param name="clientIds"></param>
        /// <param name="allowedClientIds"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        Task<PaginatedDto<Client>> GetAllAsync(string name = null, IEnumerable<string> clientIds = null, IEnumerable<string> allowedClientIds = null, int pageSize = 10, int pageIndex = 1);

        /// <summary>
        /// 获取某个客户端详情
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<Client> GetAsync(string clientId);

        /// <summary>
        /// 获取 Client 对用的 数据库实体
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="isReadonly"></param>
        /// <returns></returns>
        Task<IdentityServer4.EntityFramework.Entities.Client> GetRawClientAsync(string clientId, bool isReadonly = true);

        Task<IEnumerable<string>> GetApiResourceNamesAsync(IEnumerable<string> allowedScopes);

        Task<IEnumerable<string>> GetIdentityResourceNamesAsync(IEnumerable<string> allowedScopes);

        /// <summary>
        /// 判断 Client 是否存在
        /// </summary>
        /// <param name="clientUri"></param>
        /// <returns></returns>
        Task<bool> IsExistedAsync(string clientUri);

        /// <summary>
        /// 增加 Api 资源
        /// </summary>
        /// <param name="resources"></param>
        void AddApiResources(IEnumerable<string> resources);

        /// <summary>
        /// 增加 Client
        /// </summary>
        /// <param name="client"></param>
        void Add(Client client);

        /// <summary>
        /// 移除客户端
        /// </summary>
        /// <param name="clientIds"></param>
        /// <returns></returns>
        Task RemoveAsync(IEnumerable<string> clientIds);
    }
}
