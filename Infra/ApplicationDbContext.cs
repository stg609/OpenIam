using System;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Charlie.OpenIam.Core;
using Charlie.OpenIam.Core.Models;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Charlie.OpenIam.Infra
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>, IUnitOfWork
    {
        private const string IsDeletedProperty = "IsDeleted";
        private static readonly MethodInfo _boolPropertyMethod = typeof(EF).GetMethod(nameof(EF.Property), BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(typeof(bool));
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 锁超时时间，单位：秒
        /// </summary>
        private const int LOCK_TIMEOUT = 60 * 3;

        /// <summary>
        /// 确保对 CurrentTransaction 的操作是线程安全的
        /// </summary>
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        private IDbContextTransaction _currentTransaction;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 组织机构
        /// </summary>
        public DbSet<Organization> Organizations { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        public override int SaveChanges()
        {
            Audit(ChangeTracker, _httpContextAccessor);
            UpdateSoftDeleteStatuses(ChangeTracker);
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            Audit(ChangeTracker, _httpContextAccessor);
            UpdateSoftDeleteStatuses(ChangeTracker);
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public void PreModelCreating(DbContext dbContext, ModelBuilder modelBuilder, IHttpContextAccessor httpContextAccessor)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                NamesToSnakeCase(entity);

                // 由于这里 Entity 接受的是一个 object 类型，所以 HasQueryFilter 只能接受 LambdaExpression 参数，无法接受 lambda 表达式
                if (typeof(ISoftDeletable).IsAssignableFrom(entity.ClrType))
                {
                    entity.AddProperty(IsDeletedProperty, typeof(bool));
                    modelBuilder.Entity(entity.ClrType).HasQueryFilter(GetIsDeletedRestriction(entity.ClrType));
                }
            }

            base.OnModelCreating(modelBuilder);
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            PreModelCreating(this, builder, _httpContextAccessor);

            builder.Entity<ApplicationUser>(opts =>
            {
                opts.HasIndex(itm => itm.JobNo);
            });

            // 用户 权限 多对多
            builder.Entity<UserPermission>()
                .HasKey(t => new { t.UserId, t.PermissionId });

            builder.Entity<UserPermission>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(pt => pt.UserId);

            builder.Entity<UserPermission>()
                .HasOne(pt => pt.Permission)
                .WithMany(t => t.UserPermissions)
                .HasForeignKey(pt => pt.PermissionId);

            // 角色 权限 多对多
            builder.Entity<RolePermission>()
               .HasKey(t => new { t.RoleId, t.PermissionId });

            builder.Entity<RolePermission>()
               .HasOne(pt => pt.Role)
               .WithMany(t => t.Permissions)
               .HasForeignKey(pt => pt.RoleId);

            builder.Entity<RolePermission>()
              .HasOne(pt => pt.Permission)
              .WithMany(t => t.RolePermissions)
              .HasForeignKey(pt => pt.PermissionId);

            // 组织 用户 多对多
            builder.Entity<UserOrganization>()
               .HasKey(t => new { t.UserId, t.OrganizationId });

            builder.Entity<UserOrganization>()
               .HasOne(pt => pt.User)
               .WithMany(t => t.UserOrganizations)
               .HasForeignKey(pt => pt.UserId);

            builder.Entity<UserOrganization>()
              .HasOne(pt => pt.Organization)
              .WithMany(t => t.UserOrganizations)
              .HasForeignKey(pt => pt.OrganizationId);

            // 组织 角色 多对多
            builder.Entity<OrganizationRole>()
              .HasKey(t => new { t.RoleId, t.OrganizationId });

            builder.Entity<OrganizationRole>()
               .HasOne(pt => pt.Organization)
               .WithMany(t => t.OrganizationRoles)
               .HasForeignKey(pt => pt.OrganizationId);

            builder.Entity<OrganizationRole>()
              .HasOne(pt => pt.Role)
              .WithMany(t => t.OrganizationRoles)
              .HasForeignKey(pt => pt.RoleId);

            builder.Entity<Permission>(opts =>
            {
                opts.Property<string>("_parentId")
                   .HasColumnName("parentid");

                opts.HasOne(itm => itm.Parent)
                     .WithMany()
                     .HasForeignKey("_parentId");
            });

            builder.Entity<Organization>(opts =>
            {
                opts.Property<string>("_parentId")
                   .HasColumnName("parentid");

                opts.HasOne(itm => itm.Parent)
                     .WithMany()
                     .HasForeignKey("_parentId");
            });
        }


        private static void NamesToSnakeCase(IMutableEntityType entity)
        {
            // Replace table names
            entity.SetTableName(entity.GetTableName().ToLower());

            // Replace column names            
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToLower());
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName().ToLower());
            }

            foreach (var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName().ToLower());
            }

            foreach (var index in entity.GetIndexes())
            {
                index.SetName(index.GetName().ToLower());
            }
        }

        private static void UpdateSoftDeleteStatuses(ChangeTracker changeTracker)
        {
            foreach (var entry in changeTracker.Entries<ISoftDeletable>())
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Property(IsDeletedProperty).CurrentValue = true;
                        entry.State = EntityState.Modified;
                        break;
                }
            }
        }

        private static void Audit(ChangeTracker ChangeTracker, IHttpContextAccessor httpContextAccessor)
        {
            string userId = "Anonymous";
            var user = httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity != null)
            {
                userId = user.FindFirst(JwtClaimTypes.Subject)?.Value;
            }

            foreach (var auditable in ChangeTracker.Entries<IAuditable>())
            {
                if (auditable.State == EntityState.Added ||
                    auditable.State == EntityState.Modified)
                {
                    auditable.Property(itm => itm.LastUpdatedAt).CurrentValue = DateTime.Now;
                    auditable.Property(itm => itm.LastUpdatedBy).CurrentValue = userId;

                    if (auditable.State == EntityState.Added)
                    {
                        auditable.Property(itm => itm.CreatedAt).CurrentValue = DateTime.Now;
                        auditable.Property(itm => itm.CreatedBy).CurrentValue = userId;
                    }
                    else
                    {
                        auditable.Property(itm => itm.CreatedAt).IsModified = false;
                        auditable.Property(itm => itm.CreatedBy).IsModified = false;
                    }
                }
            }
        }

        private static LambdaExpression GetIsDeletedRestriction(Type type)
        {
            var parm = Expression.Parameter(type, "it");
            var prop = Expression.Call(_boolPropertyMethod, parm, Expression.Constant(IsDeletedProperty));
            var condition = Expression.MakeBinary(ExpressionType.Equal, prop, Expression.Constant(false));
            var lambda = Expression.Lambda(condition, parm);
            return lambda;
        }
    }
}
