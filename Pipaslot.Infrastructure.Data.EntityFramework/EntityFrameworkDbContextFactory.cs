using System;
using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    public class EntityFrameworkDbContextFactory : EntityFrameworkDbContextFactory<DbContext>, IEntityFrameworkDbContextFactory
    {
        public EntityFrameworkDbContextFactory(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }
    }

    public class EntityFrameworkDbContextFactory<TDbContext> : IEntityFrameworkDbContextFactory<TDbContext>
        where TDbContext : DbContext
    {
        private readonly DbContextOptions _dbContextOptions;


        public EntityFrameworkDbContextFactory(DbContextOptions dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public TDbContext Create()
        {
            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), _dbContextOptions);
        }

        public TDbContext GetReadOnlyContext()
        {
            var context = Create();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return context;
        }
    }
}
