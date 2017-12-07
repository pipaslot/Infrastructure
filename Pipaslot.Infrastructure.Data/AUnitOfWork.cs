using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Abstract base Unit of Work
    /// </summary>
    public abstract class AUnitOfWork : IUnitOfWork
    {
        private readonly List<Action> onCommitActions = new List<Action>();

        private bool isDisposed;

        public event EventHandler Disposing;
        
        public virtual void Commit()
        {
            CommitCore();

            FireOnCommitActions();
        }

        public virtual async Task CommitAsync()
        {
            await CommitAsync(default(CancellationToken));
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await CommitAsyncCore(cancellationToken);

            FireOnCommitActions();
        }

        public void OnCommit(Action action)
        {
            onCommitActions.Add(action);
        }


        /// <inheritdoc />
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                Disposing?.Invoke(this, EventArgs.Empty);
                DisposeCore();
            }
        }

        private void FireOnCommitActions()
        {
            foreach (var action in onCommitActions)
            {
                action();
            }
            onCommitActions.Clear();
        }

        /// <summary>
        /// Performs the real commit work.
        /// </summary>
        protected abstract void CommitCore();

        /// <summary>
        /// Performs the real asynchronously commit work.
        /// </summary>
        /// <param name="cancellationToken"></param>
        protected abstract Task CommitAsyncCore(CancellationToken cancellationToken);

        /// <summary>
        /// Performs the real dispose work.
        /// </summary>
        protected abstract void DisposeCore();
    }
}
