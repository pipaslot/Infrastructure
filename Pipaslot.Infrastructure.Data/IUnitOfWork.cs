using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Data
{
    /// <inheritdoc />
    /// <summary>
    /// An interface that represents a boundary of a business transaction.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commits the changes made inside this unit of work.
        /// </summary>
        void Commit();

        /// <summary>
        /// Asynchronously commits the changes made inside this unit of work.
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// Asynchronously commits the changes made inside this unit of work.
        /// </summary>
        Task CommitAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Action callef after commit
        /// </summary>
        void OnCommit(Action action);

        /// <summary>
        /// Occurs when this unit of work is disposed.
        /// </summary>
        event EventHandler Disposing;
    }
}
