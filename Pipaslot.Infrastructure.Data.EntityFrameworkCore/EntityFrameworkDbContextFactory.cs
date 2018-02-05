using System;
using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCore
{
    /// <inheritdoc cref="IEntityFrameworkDbContextFactory" />
    /// <summary>
    /// DB Factory for three contexts
    /// </summary>
    /// <typeparam name="TDbContext1"></typeparam>
    /// <typeparam name="TDbContext2"></typeparam>
    /// <typeparam name="TDbContext3"></typeparam>
    public class EntityFrameworkDbContextFactory<TDbContext1, TDbContext2, TDbContext3> : EntityFrameworkDbContextFactory<TDbContext1, TDbContext2>, IEntityFrameworkDbContextFactory<TDbContext3>
        where TDbContext1 : DbContext
        where TDbContext2 : DbContext
        where TDbContext3 : DbContext
    {
        private readonly DbContextOptions _dbContextOptions3;

        public EntityFrameworkDbContextFactory(DbContextOptions dbContextOptions1, DbContextOptions dbContextOptions2, DbContextOptions dbContextOptions3) : base(dbContextOptions1, dbContextOptions2)
        {
            _dbContextOptions3 = dbContextOptions3;
        }

        public new virtual TDbContext3 GetReadOnlyContext()
        {
            return GetReadOnlyContextCore<TDbContext3>(_dbContextOptions3);
        }

        public new virtual TDbContext3 Create()
        {
            return CreateCore<TDbContext3>(_dbContextOptions3);
        }
    }

    /// <inheritdoc cref="IEntityFrameworkDbContextFactory" />
    /// <summary>
    /// DB Factory for two contexts
    /// </summary>
    /// <typeparam name="TDbContext1"></typeparam>
    /// <typeparam name="TDbContext2"></typeparam>
    public class EntityFrameworkDbContextFactory<TDbContext1, TDbContext2> : EntityFrameworkDbContextFactory<TDbContext1>, IEntityFrameworkDbContextFactory<TDbContext2>
        where TDbContext1 : DbContext
        where TDbContext2 : DbContext
    {
        private readonly DbContextOptions _dbContextOptions2;

        public EntityFrameworkDbContextFactory(DbContextOptions dbContextOptions1, DbContextOptions dbContextOptions2) : base(dbContextOptions1)
        {
            _dbContextOptions2 = dbContextOptions2;
        }

        public new virtual TDbContext2 GetReadOnlyContext()
        {
            return GetReadOnlyContextCore<TDbContext2>(_dbContextOptions2);
        }

        public new virtual TDbContext2 Create()
        {
            return CreateCore<TDbContext2>(_dbContextOptions2);
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// DB Factory for single context
    /// </summary>
    /// <typeparam name="TDbContext1"></typeparam>
    public class EntityFrameworkDbContextFactory<TDbContext1> : IEntityFrameworkDbContextFactory<TDbContext1>
        where TDbContext1 : DbContext
    {
        private readonly DbContextOptions _dbContextOptions1;

        public EntityFrameworkDbContextFactory(DbContextOptions dbContextOptions1)
        {
            _dbContextOptions1 = dbContextOptions1;
        }
        
        public virtual TDbContext1 GetReadOnlyContext()
        {
            return GetReadOnlyContextCore<TDbContext1>(_dbContextOptions1);
        }

        public virtual TDbContext1 Create()
        {
            return CreateCore<TDbContext1>(_dbContextOptions1);
        }

        #region Factory helpers

        /// <summary>
        /// Reusable Read-Only Context factory
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        protected virtual TDbContext GetReadOnlyContextCore<TDbContext>(DbContextOptions options)where TDbContext : DbContext
        {
            var context = CreateCore<TDbContext>(options);
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return context;
        }

        /// <summary>
        /// Reusable Context factory
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="option"></param>
        /// <returns></returns>
        protected virtual TDbContext CreateCore<TDbContext>(DbContextOptions option) where TDbContext : DbContext
        {
            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), _dbContextOptions1);
        }
        
        #endregion

        #region Generic Implementation of top Factory interface

        public TDbContext Create<TDbContext>() where TDbContext : DbContext
        {
            return GetFactory<TDbContext>().Create();
        }

        public TDbContext GetReadOnlyContext<TDbContext>() where TDbContext : DbContext
        {
            return GetFactory<TDbContext>().GetReadOnlyContext();
        }

        protected IEntityFrameworkDbContextFactory<TDbContext> GetFactory<TDbContext>()
            where TDbContext : DbContext
        {
            if (this is IEntityFrameworkDbContextFactory<TDbContext> f)
            {
                return  f;
            }
            var factoryName = this.GetType().FullName;
            var contextName = typeof(TDbContext).FullName;
            throw new InvalidOperationException($"DbFactory: {factoryName} does not contains factory implementation for DbContext: {contextName}");
        }

        #endregion
    }
}
