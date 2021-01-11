using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Charlie.OpenIam.Core;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Charlie.OpenIam.Infra
{
    public class IamConfigurationDbContext:
        ConfigurationDbContext<IamConfigurationDbContext>, IUnitOfWork
    {
        /// <summary>
        /// 锁超时时间，单位：秒
        /// </summary>
        private const int LOCK_TIMEOUT = 60 * 3;

        /// <summary>
        /// 确保对 CurrentTransaction 的操作是线程安全的
        /// </summary>
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        private IDbContextTransaction _currentTransaction;

        public IamConfigurationDbContext(DbContextOptions<IamConfigurationDbContext> options,
               ConfigurationStoreOptions storeOptions)
               : base(options, storeOptions)
        {
        }

        /// <summary>
        /// 提交工作单元
        /// </summary>
        /// <param name="transactionId">事务唯一编号</param>
        /// <returns></returns>
        public async Task CommitAsync(string transactionId)
        {
            try
            {
                await _lock.WaitAsync(TimeSpan.FromSeconds(LOCK_TIMEOUT));

                if (_currentTransaction != null && _currentTransaction.TransactionId.ToString() != transactionId)
                {
                    throw new InvalidOperationException($"Transaction {transactionId} is not current");
                }

                //没有考虑 基于 DbUpdateConcurrencyException 的 retry，是担心会覆盖别人的数据
                try
                {
                    await SaveChangesAsync();
                    _currentTransaction?.Commit();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    //并发错误

                    Rollback();
                    throw new IamException(System.Net.HttpStatusCode.InternalServerError, "并发错误", ex);
                }
                catch
                {
                    Rollback();
                    throw;
                }
                finally
                {
                    if (_currentTransaction != null)
                    {
                        _currentTransaction.Dispose();
                        _currentTransaction = null;
                    }
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns></returns>
        public async Task<(IDisposable Transaction, string TransactionId)> BeginAsync()
        {
            try
            {
                await _lock.WaitAsync(TimeSpan.FromSeconds(LOCK_TIMEOUT));

                if (_currentTransaction != null)
                {
                    return (null, String.Empty);
                }

                _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

                return (_currentTransaction, _currentTransaction.TransactionId.ToString());
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="transactionId">事务唯一编号</param>
        public void Rollback()
        {
            try
            {
                _lock.Wait(TimeSpan.FromSeconds(LOCK_TIMEOUT));

                try
                {
                    _currentTransaction?.Rollback();
                }
                finally
                {
                    if (_currentTransaction != null)
                    {
                        _currentTransaction.Dispose();
                    }
                }
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
