using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCore
{
    class EntityFrameworkRepositoryQuery<TDbContext, TResult> : EntityFrameworkQuery<TDbContext, TResult, TResult>, IQuery<TResult>
            where TDbContext : DbContext
            where TResult : class

    {
        public EntityFrameworkRepositoryQuery(IEntityFrameworkDbContextFactory dbContextFactory) : base(dbContextFactory)
        {
        }

        protected override IEnumerable<TResult> PostProcessResults(IEnumerable<TResult> results)
        {
            return results;
        }

        protected override IQueryable<TResult> GetQueryable()
        {
            return ContextReadOnly.Set<TResult>();
        }
    }
}
