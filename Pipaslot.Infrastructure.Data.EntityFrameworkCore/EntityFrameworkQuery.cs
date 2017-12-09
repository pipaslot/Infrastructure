using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCore
{
    public abstract class EntityFrameworkQuery<TResult, TDbContext> : EntityFrameworkQuery<TResult, TResult, TDbContext>
        where TDbContext : DbContext

    {
        protected EntityFrameworkQuery(EntityFrameworkUnitOfWorkFactory<TDbContext> uowFactory) : base(uowFactory)
        {
        }

        /// <summary>
        ///     When overriden in derived class, it allows to modify the materialized results of the query before they are returned
        ///     to the caller.
        /// </summary>
        protected override IList<TResult> PostProcessResults(IList<TResult> results)
        {
            return results;
        }
    }

    public abstract class EntityFrameworkQuery<TQueryableResult, TResult, TDbContext> : Query<TQueryableResult, TResult>
    where TDbContext : DbContext
    {
        private readonly EntityFrameworkUnitOfWorkFactory<TDbContext> _uowFactory;

        protected EntityFrameworkQuery(EntityFrameworkUnitOfWorkFactory<TDbContext> uowFactory)
        {
            _uowFactory = uowFactory;
        }

        /// <summary>
        /// Context for Read only operations. Unit of work is not needed for this operation. If Unit of work does not exists, then is created a new context
        /// </summary>
        protected TDbContext ContextReadOnly => _uowFactory.GetReadOnlyContext();


        public override async Task<int> GetTotalRowCountAsync(CancellationToken cancellationToken)
        {
            return await GetQueryable().CountAsync(cancellationToken);
        }


        protected override async Task<IList<TQueryableResult>> ExecuteQueryAsync(IQueryable<TQueryableResult> query, CancellationToken cancellationToken)
        {
            return await query.ToListAsync(cancellationToken);
        }
    }
}
