using System.Collections.Generic;
using System.Threading.Tasks;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Core.Services.Dtos;

namespace Charlie.OpenIam.Core.Services.Abstractions
{
    /// <summary>
    /// 客户端服务
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// 获取所有
        /// </summary>
        /// <param name="name"></param>
        /// <param name="clientIds"></param>
        /// <param name="allowedClientIds"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        Task<PaginatedDto<ClientDto>> GetAllAsync(string name = null, IEnumerable<string> clientIds = null, IEnumerable<string> allowedClientIds = null, int pageSize = 10, int pageIndex = 0);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task<ClientDto> GetAsync(string clientId, IEnumerable<string> allowedClientIds);

        /// <summary>
        /// 创建客户端
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<(string ClientId, string PlainSecret)> CreateAsync(ClientNewDto model);

        /// <summary>
        /// 更新客户端
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="model"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task UpdateAsync(string clientId, ClientUpdateDto model, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="isEnabled"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task SwitchAsync(string clientId, bool isEnabled, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 刷新客户端密钥
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task<string> ResetSecretAsync(string clientId, IEnumerable<string> allowedClientIds = null);

        /// <summary>
        /// 移除客户端
        /// </summary>
        /// <param name="targetClientIds"></param>
        /// <param name="allowedClientIds"></param>
        /// <returns></returns>
        Task RemoveAsync(IEnumerable<string> targetClientIds, IEnumerable<string> allowedClientIds = null);
    }
}
