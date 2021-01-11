using System;
using System.Threading.Tasks;
using Charlie.OpenIam.Core;

namespace Charlie.OpenIam.Web.Infra
{
    /// <summary>
    /// 空的 UnitOfWork，只用于不需要 UnitOfWork 的场景
    /// </summary>
    public class EmptyUnitOfWork : IUnitOfWork
    {
        public Task<(IDisposable Transaction, string TransactionId)> BeginAsync()
        {
            return Task.FromResult<(IDisposable Transaction, string TransactionId)>((null, null));
        }

        public Task CommitAsync(string transactionId)
        {
            return Task.CompletedTask;
        }

        public void Rollback()
        {
            // 空的方法
        }
    }
}
