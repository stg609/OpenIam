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
        Task<PaginatedDto<ApplicationUser>> GetAllAsync(string firstName = null, string lastName = null, string jobNo = null, string idcard = null, string phone = null, string email = null, string excludeOrgId = null, bool? isActive = null, int pageSize = 10, int pageIndex = 0);

        /// <summary>
        /// 获取某个用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jobNo"></param>
        /// <param name="isReadonly"></param>
        /// <returns></returns>
        Task<ApplicationUser> GetAsync(string id = null, string jobNo = null, bool isReadonly = true);
    }
}
