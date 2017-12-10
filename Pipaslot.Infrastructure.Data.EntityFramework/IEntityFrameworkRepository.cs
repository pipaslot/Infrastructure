using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    /// <inheritdoc />
    /// <summary>
    /// An interface for repository in Entity Framework.
    /// </summary>
    public interface IEntityFrameworkRepository<TEntity, in TKey, TDbContext> : IRepository<TEntity, TKey>
        where TEntity : IEntity<TKey>
        where TDbContext : DbContext
    {
    }
}