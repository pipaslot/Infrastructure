using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    /// <summary>
    /// Created Database contex for read-write
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public interface IEntityFrameworkDbContextFactory<TDbContext>
        where TDbContext : DbContext
    {
        /// <summary>
        /// Create Read-Write context for unit of work
        /// </summary>
        /// <returns></returns>
        TDbContext Create();
    }
}
