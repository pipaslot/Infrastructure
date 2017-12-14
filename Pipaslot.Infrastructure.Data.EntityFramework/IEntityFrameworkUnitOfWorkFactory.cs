using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    public interface IEntityFrameworkUnitOfWorkFactory<out TDbContext> : IUnitOfWorkFactory
        where TDbContext : DbContext
    {
        /// <summary>
        /// Creates an unit of work scope.
        /// </summary>
        IEntityFrameworkUnitOfWork<TDbContext> Create();

        /// <summary>
        /// Get one Existing Unit of work on requested level of nesting
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IEntityFrameworkUnitOfWork<TDbContext> GetCurrent(int index = 0);
    }

    //TODo ODSTRANIT ???
    public interface IEntityFrameworkUnitOfWorkFactory : IUnitOfWorkFactory
    {
        /// <summary>
        /// Creates an unit of work scope.
        /// </summary>
        IEntityFrameworkUnitOfWork Create();

        /// <summary>
        /// Get one Existing Unit of work on requested level of nesting
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IEntityFrameworkUnitOfWork GetCurrent(int index = 0);
    }
}