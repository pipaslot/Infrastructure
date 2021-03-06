﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCore
{
    public abstract class EntityFrameworkQuery<TDbContext, TResult> : EntityFrameworkQuery<TDbContext, TResult, TResult>, IQuery<TResult>
        where TDbContext : DbContext
        where TResult : class

    {
        protected EntityFrameworkQuery(IEntityFrameworkDbContextFactory dbContextFactory) : base(dbContextFactory)
        {
        }

        protected override IEnumerable<TResult> PostProcessResults(IEnumerable<TResult> results)
        {
            return results;
        }
    }

    public abstract class EntityFrameworkQuery<TDbContext, TQueryableResult, TResult> : AQuery<TQueryableResult, TResult>
        where TDbContext : DbContext
        where TQueryableResult : class
    {
        private readonly IEntityFrameworkDbContextFactory _dbContextFactory;

        protected EntityFrameworkQuery(IEntityFrameworkDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Context for Read only operations. Unit of work is not needed for this operation. If Unit of work does not exists, then is created a new context
        /// </summary>
        protected TDbContext ContextReadOnly => _dbContextFactory.GetReadOnlyContext<TDbContext>();
        
        public override async Task<int> GetTotalRowCountAsync(CancellationToken cancellationToken)
        {
            return await GetQueryable().CountAsync(cancellationToken);
        }


        protected override async Task<IEnumerable<TQueryableResult>> ExecuteQueryAsync(IQueryable<TQueryableResult> query, CancellationToken cancellationToken)
        {
            return await query.ToListAsync(cancellationToken);
        }
    }
}
