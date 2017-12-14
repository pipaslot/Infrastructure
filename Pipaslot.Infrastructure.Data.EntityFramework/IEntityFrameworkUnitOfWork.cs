using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    public interface IEntityFrameworkUnitOfWork<out TDbContext> : IEntityFrameworkUnitOfWork
        where TDbContext : DbContext
    {
        new TDbContext Context { get; }
    }

    public interface IEntityFrameworkUnitOfWork : IUnitOfWork
    {
        DbContext Context { get; }
    }
}