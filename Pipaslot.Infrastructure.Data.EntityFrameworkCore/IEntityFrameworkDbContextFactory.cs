using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCore
{
    public interface IEntityFrameworkDbContextFactory<out TDbContext> : IEntityFrameworkDbContextFactory
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

    /// <summary>
    /// Created Database context
    /// </summary>
    public interface IEntityFrameworkDbContextFactory
    {
        /// <summary>
        /// Create Read-Write context for unit of work
        /// </summary>
        /// <returns></returns>
        TDbContext Create<TDbContext>() where TDbContext : DbContext;

        /// <summary>
        /// Create Read only context for Queries
        /// </summary>
        /// <returns></returns>
        TDbContext GetReadOnlyContext<TDbContext>() where TDbContext : DbContext;
    }
}
