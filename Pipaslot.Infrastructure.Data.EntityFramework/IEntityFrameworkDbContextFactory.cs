using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    public interface IEntityFrameworkDbContextFactory : IEntityFrameworkDbContextFactory<DbContext>
    {
        
    }

    /// <summary>
    /// Created Database contex for read-write
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public interface IEntityFrameworkDbContextFactory<out TDbContext>
        where TDbContext : DbContext
    {
        /// <summary>
        /// Create Read-Write context for unit of work
        /// </summary>
        /// <returns></returns>
        TDbContext Create();
        
        /// <summary>
        /// Create Read only context for Queries
        /// </summary>
        /// <returns></returns>
        TDbContext GetReadOnlyContext();
    }
}
