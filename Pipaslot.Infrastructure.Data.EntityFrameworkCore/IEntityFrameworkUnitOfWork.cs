using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCore
{
    /// <inheritdoc />
    /// <summary>
    /// Unit of work which can access DbContext
    /// </summary>
    public interface IEntityFrameworkUnitOfWork<out TDbContext> : IUnitOfWork
        where TDbContext : DbContext
    {
        TDbContext Context { get; }
    }
}