using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCore
{
    public class EntityFrameworkUnitOfWork<TDbContext> : AUnitOfWork where TDbContext : DbContext
    {
        private readonly IDbContextTransaction _topLevelTransaction;

        /// <summary>
        /// Gets the <see cref="DbContext"/>.
        /// </summary>
        public TDbContext Context { get; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sharedDbContext"></param>
        /// <param name="topLevelTransaction">Transaction scope assigned only for the highest Unit of Wofk in nesting</param>
        public EntityFrameworkUnitOfWork(TDbContext sharedDbContext, IDbContextTransaction topLevelTransaction = null)
        {
            _topLevelTransaction = topLevelTransaction;
            Context = sharedDbContext;
        }

        protected override void CommitCore()
        {
            Context.SaveChanges();
            _topLevelTransaction?.Commit();
        }

        protected override async Task CommitAsyncCore(CancellationToken cancellationToken)
        {
            await Context.SaveChangesAsync(cancellationToken);
            _topLevelTransaction?.Commit();
        }

        /// <inheritdoc />
        /// <summary>
        /// Dispose context only if this unit of work is on top of scope nesting
        /// </summary>
        protected override void DisposeCore()
        {
            if(_topLevelTransaction != null)
            {
                Context.Dispose();
                _topLevelTransaction.Dispose();
            }
        }
    }
}
