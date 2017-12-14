using System;
using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFramework
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

        public new TDbContext3 Create()
        {
            return (TDbContext3)Activator.CreateInstance(typeof(TDbContext1), _dbContextOptions3);
        }

        public new TDbContext3 GetReadOnlyContext()
        {
            var context = Create();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return context;
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

        public new TDbContext2 Create()
        {
            return (TDbContext2)Activator.CreateInstance(typeof(TDbContext1), _dbContextOptions2);
        }

        public new TDbContext2 GetReadOnlyContext()
        {
            var context = Create();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return context;
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


        public TDbContext1 Create()
        {
            return (TDbContext1)Activator.CreateInstance(typeof(TDbContext1), _dbContextOptions1);
        }

        public TDbContext1 GetReadOnlyContext()
        {
            var context = Create();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return context;
        }

        #region Generic Implementation

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
