using System;
using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    public class EntityFrameworkDbContextFactory<TDbContext> : IEntityFrameworkDbContextFactory<TDbContext>
        where TDbContext : DbContext
    {
        private readonly DbContextOptions<TDbContext> _dbContextOptions;


        public EntityFrameworkDbContextFactory(DbContextOptions<TDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public TDbContext Create()
        {
            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), _dbContextOptions);
        }
    }
}
