using System.Threading.Tasks;
using Charlie.OpenIam.Core.Models.Services.Dtos;

namespace Charlie.OpenIam.Core.Services.Abstractions
{
    /// <summary>
    /// 系统 服务
    /// </summary>
    public interface ISysService
    {
        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <returns></returns>
        Task<SysDto> GetAsync();

        /// <summary>
        /// 更新系统信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpdateAsync(SysDto model);
    }
}
