using System.Threading.Tasks;

namespace Charlie.OpenIam.Core.Models.Repositories
{
    /// <summary>
    /// 系统信息 仓储接口
    /// </summary>
    public interface ISysRepo
    {
        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <returns></returns>
        Task<SystemInfo> GetAsync(bool isReadonly = true);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        void Add(SystemInfo model);
    }
}
