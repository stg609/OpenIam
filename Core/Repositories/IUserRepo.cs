using System.Threading.Tasks;
using Charlie.OpenIam.Common;

namespace Charlie.OpenIam.Core.Models.Repositories
{
    /// <summary>
    /// 用户 仓储接口
    /// </summary>
    public interface IUserRepo
    {
        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jobNo"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<bool> IsExistedAsync(string id = null, string jobNo = null, string phone = null);

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
        /// 获取所有用户
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="jobNo"></param>
        /// <param name="idcard"></param>
        /// <param name="phone">手机号，模糊匹配</param>
        /// <param name="email"></param>
        /// <param name="excludeOrgId"></param>
        /// <param name="isActive"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        Task<PaginatedDto<ApplicationUser>> GetAllAsync(string firstName = null, string lastName = null, string jobNo = null, string idcard = null, string phone = null, string email = null, string excludeOrgId = null, bool? isActive = null, int pageSize = 10, int pageIndex = 0);

        /// <summary>
        /// 获取某个用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jobNo">工号，严格匹配</param>
        /// <param name="phone">手机号，严格匹配</param>
        /// <param name="isReadonly"></param>
        /// <returns></returns>
        Task<ApplicationUser> GetAsync(string id = null, string jobNo = null, string phone = null, bool isReadonly = true);
    }
}
