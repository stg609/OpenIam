using System.Collections.Generic;
using System.Threading.Tasks;

namespace Charlie.OpenIam.Core.Models.Repositories
{
    /// <summary>
    /// 组织 仓储接口
    /// </summary>
    public interface IOrgRepo
    {
        /// <summary>
        /// 增加组织
        /// </summary>
        /// <param name="organization"></param>
        void Add(Organization organization);

        /// <summary>
        /// 获取所有组织
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        Task<IEnumerable<Organization>> GetAllAsync(string name = null, bool? isEnabled = null);

        /// <summary>
        /// 获取单个组织详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isReadonly"></param>
        /// <returns></returns>
        Task<Organization> GetAsync(string id, bool isReadonly = true);

        /// <summary>
        /// 删除组织
        /// </summary>
        /// <param name="userId">当前用户</param>
        /// <param name="targetIds">要删除的组织编号</param>
        /// <returns>删除的组织编号</returns>
        Task<IEnumerable<string>> RemoveAsync(string userId, IEnumerable<string> targetIds);
    }
}
