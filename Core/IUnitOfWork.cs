using System;
using System.Threading.Tasks;

namespace Charlie.OpenIam.Core
{
    /// <summary>
    /// 工作单元（线程安全）
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 提交工作单元
        /// </summary>
        /// <param name="transactionId">事务唯一编号</param>
        /// <returns></returns>
        Task CommitAsync(string transactionId);

        /// <summary>
        /// 开始事务, 如果当前 db 实例 已经启动了一个事务，则不会开启新的 Transaction, 返回默认值
        /// </summary>
        /// <returns></returns>
        Task<(IDisposable Transaction, string TransactionId)> BeginAsync();

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="transactionId">事务唯一编号</param>
        void Rollback();
    }
}
